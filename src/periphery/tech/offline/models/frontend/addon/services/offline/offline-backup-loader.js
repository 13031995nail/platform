import $ from 'jquery';
import { A } from '@ember/array';
import { Promise } from 'rsvp';
import { isNone, isBlank } from '@ember/utils';
import { getOwner } from '@ember/application';
import EmberObject from '@ember/object';
import Service, { inject as service } from '@ember/service';
import { SimplePredicate, ComplexPredicate, DatePredicate }
  from 'ember-flexberry-data/query/predicate';
import config from 'ember-get-config';
import strings from '../../utils/strings';``
import Moment from 'moment';
import PredicatSerializer from '../../serializers/predicate-serializer';
import { modelClassForType }
  from '@timepath/tmobis-core/utils/model-path-for-type';
import QueryBuilder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';

export default Service.extend(PredicatSerializer, {
  init() {
    this._super(...arguments);
  },

  /**
   * Сервис для работы с файлами.
   */
  file: service(),

  dexie: service(),
  store: service(),
  session: service(),
  modelNameConverter: service('model-name-converter'),
  offlineEntities: service('offline/offline-entities'),
  
  loadingDictionariesVisible: false,
  loadingDictionaries: undefined,
  loadingDictionariesTotalProgress: 0,
  loadingBackupsVisible: false,
  loadingBackups: undefined,
  loadingBackupsTotalProgress: 0,
  loadErrors: undefined,
  fullLoadErrors: undefined,
  syncObjsErrors: undefined,

  clearDictionariesForReload(controller) {
    let self = this,
      objs = self.get('offlineEntities.backupEntities'),
      store = controller.store,
      localStore = getOwner(controller).lookup('store:local');
    return self.getOfflineLoadObjects(store).then(results => {
      let ofllineLoadObjsModel = 'offline-load-object',
        localAdapter = localStore.adapterFor(ofllineLoadObjsModel),
        promises = [localAdapter.clear(ofllineLoadObjsModel)];
      objs.forEach(function(obj) {
        let offlineObj = results.filter(function(item) {
          return item.get('objectsModelName') === obj.modelName;
        }).get('firstObject');
        if (!isNone(offlineObj)) {
          let localAdapter = localStore.adapterFor(obj.modelName);
          promises.push(localAdapter.clear(obj.modelName));
        }
      }, this);
      return Promise.all(promises).then(() => Promise.resolve(true));
    }).catch(e => Promise.resolve(false));
  },

  checkLoadedObjects(store, offlineGlobals) {
    let self = this,
      objs = this.get('offlineEntities.backupEntities');
    return self.getOfflineLoadObjects(store).then(results => {
      let notLoaded = A();
      return offlineGlobals.checkBackendAvailable().then(isOnline => {
        if (isOnline) {
          let addNewItem = function(obj) {
            let newItem = EmberObject.create(obj);
            newItem.set('progress', 0);
            newItem.set('error', '');
            notLoaded.pushObject(newItem);
          };

          if (results.get('length') > 0) {
            objs.forEach(function(obj) {
              if ((!isBlank(config.APP.offline.alwaysLoadedOnLoginObjects) &&
                config.APP.offline.alwaysLoadedOnLoginObjects.some(function(item) {
                  return item === obj.modelName;
                })) ||
                !results.any(function(item) {
                  return item.get('objectsModelName') === obj.modelName && item.get('objectsIsLoaded');
                })) {
                addNewItem(obj);
              }
            }, this);
          } else {
            objs.forEach(function(obj) {
              addNewItem(obj);
            }, this);
          }
        }

        return { notLoaded: notLoaded, objects: results };
      }).catch(e => ({ notLoaded: notLoaded, objects: results }));
    });
  },

  getLoadingObjects(store, offlineGlobals, forFullLoad, fullLoadModels) {
    let self = this,
      objs = this.get('offlineEntities.backupEntities');
    return self.getOfflineLoadObjects(store).then(results => {
      let loadingData = A();
      return offlineGlobals.checkBackendAvailable().then(isOnline => {
        if (isOnline) {
          let addNewItem = function(obj, successLoadDate, loadDate, editDate, isFirstLoadCompleted) {
            let newItem = EmberObject.create(obj);
            newItem.set('progress', 0);
            newItem.set('error', '');
            newItem.set('loadDate', loadDate);
            newItem.set('lastSuccessLoadDate', successLoadDate);
            newItem.set('editDate', editDate);
            newItem.set('isFirstLoadCompleted', isFirstLoadCompleted);
            let netDistrictPaths = newItem.get('netDistrictPaths'),
              netDistrictIds = newItem.get('netDistrictIds'),
              townPaths = newItem.get('townPaths');
            if (!isNone(netDistrictPaths)) {
              let predicate = new SimplePredicate('id', FilterOperator.Eq, null);
              if (!isNone(netDistrictIds)) {
                let byPathsPredicates = netDistrictPaths.map(x => {
                  let byNetDistrictsPredicates = netDistrictIds.map(y => new SimplePredicate(x, FilterOperator.Eq, y));

                  return byNetDistrictsPredicates.length === 1 ?
                    byNetDistrictsPredicates[0] :
                    new ComplexPredicate(Condition.Or, ...byNetDistrictsPredicates);
                });
                predicate = byPathsPredicates.length === 1 ? byPathsPredicates[0] : new ComplexPredicate(Condition.And, ...byPathsPredicates);
              }
              newItem.set('predicate', predicate);
            } else if (!isNone(townPaths)) {
              let predicate = new SimplePredicate(townPaths, FilterOperator.Eq, self.get('session.data.authenticated.town'));
              newItem.set('predicate', predicate);
            }

            loadingData.pushObject(newItem);
          };

          if (results.get('length') > 0) {
            objs.forEach(function(obj) {
              if (forFullLoad) {
                if (!isBlank(config.APP.offline.alwaysLoadedOnLoginObjects) &&
                  config.APP.offline.alwaysLoadedOnLoginObjects.some(function(item) {
                    return item === obj.modelName;
                  }) ||
                  !isBlank(fullLoadModels) && fullLoadModels.some(function(item) {
                    return item === obj.modelName;
                  })) {
                  addNewItem(obj);
                } else {
                  let offlineObj = results.filter(function(item) {
                    return item.get('objectsModelName') === obj.modelName;
                  }).get('firstObject');
                  if (isNone(offlineObj) || !offlineObj.get('objectsIsLoaded')) {
                    addNewItem(obj, isNone(offlineObj) ? null : offlineObj.get('objectsLastSuccessLoadDate'));
                  }
                }
              } else {
                let offlineObj = results.filter(function(item) {
                  return item.get('objectsModelName') === obj.modelName;
                }).get('firstObject');
                if (!obj.onlyFirstLoad && !isNone(offlineObj)) {
                  addNewItem(obj, offlineObj.get('objectsLastSuccessLoadDate'), offlineObj.get('objectsLoadDate'), offlineObj.get('objectsLoadByEditDate'), offlineObj.get('objectsIsLoaded'));
                }
              }
            }, this);
          } else {
            objs.forEach(function(obj) {
              addNewItem(obj);
            }, this);
          }
        }

        return { loadingObjects: loadingData, offlineObjectsData: results };
      }).catch(e => ({ loadingObjects: loadingData, offlineObjectsData: results }));
    });
  },

  getCheckingObjects(store, offlineGlobals) {
    try {
      let openDb = indexedDB.open('tmobis');
      return new Promise(function(resolve, reject) {
        openDb.onsuccess = function() {
          resolve(openDb.result);
        };

        openDb.onerror = function() {
          resolve(null);
        };
      }).then(db => {
        if (db) {
          let data = [
              { modelName: 'armti-building-input' },

              //{ modelName: 'armti-building' },
              //{ modelName: 'armti-consumption-object' },
              //{ modelName: 'armti-supply-thermal-unit' }
            ],
            promises = data.map(function(item) {
              return new Promise(function(resolveTr, rejectTr) {
                let r = db.transaction([item.modelName], 'readwrite').objectStore(item.modelName).getAllKeys();
                r.onsuccess = function() {
                  let idsStr = JSON.stringify(r.result),
                    compressed = strings.lzw_compress(idsStr);
                  resolveTr({
                    ModelName: modelClassForType(item.modelName),
                    LzwPks: compressed
                  });
                };

                r.onerror = function() {
                  resolveTr(null);
                };
              });
            });
          return Promise.all(promises);
        } 
          return Promise.resolve([]);
        
      });

    } catch (e) {
      return Promise.resolve([]);
    }
  },

  getObjsForDeleteFromServer(store, offlineGlobals) {
    let self = this,
        modelNameConverter = this.get('modelNameConverter'),
        curDate = new Moment();
    return self.getCheckingObjects(store, offlineGlobals).then(loadingData => {
      let fixDate = new Moment();
      console.debug('Получение всех id: ' + fixDate.diff(curDate, 'ms') + ' мс');
      return offlineGlobals.checkBackendAvailable().then(isOnline => new Promise(function(resolve, reject) {
          if (isOnline) {
            let token = self.get('session.data.authenticated.token') || self.get('session.data.authenticated.access_token'),
              message = 'Ошибка при загрузке с сервера информации по удаленным объектам.';
            fixDate = new Moment();
            $.ajax({
              url: config.APP.backendUrl + '/api/UpdatingObjects/CheckDeletedObjects',
              data: JSON.stringify(loadingData),
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              type: 'POST',
              headers: { Authorization: 'Bearer ' + token },
              success: function(data) {
                if (data) {
                  let jsonData;
                  try {
                    jsonData = JSON.parse(data);
                    fixDate = new Moment();
                    console.debug('Получение ключей отсутствующих на сервере: ' + fixDate.diff(fixDate, 'ms') + ' мс');
                    console.debug('Итого: ' + fixDate.diff(curDate, 'ms') + ' мс');
                  } catch (e) {
                    let fError = 'Неверный формат полученных с сервера данных.';
                    console.error(fError + '\n' + data, e);
                    resolve({ data: [], error: fError });
                    return;
                  }

                  if (isBlank(jsonData.error)) {
                    let modelsForDelete = jsonData.data.map(function(item) {
                      return { 
                        modelName: modelNameConverter
                          .modelNameByClass(item.ModelName),
                        pks: item.Pks };
                    });
                    resolve({ data: modelsForDelete });
                  } else {
                    resolve({ data: [], error: message });
                  }
                } else {
                  resolve({ data: [], error: message });
                }
              },
              error: function(e) {
                console.error(message, e);
                resolve({ error: message });
              }
            });
          } else {
            resolve({ data: [] });
          }
        })).catch(e => Promise.resolve({ data: [], offlineObjectsData: loadingData.offlineObjectsData }));
    }).catch(e => Promise.resolve({ data: [] }));
  },

  getDictsForFullLoad(store, offlineGlobals, fullLoadModels) {
    let self = this;
    return self.getLoadingObjects(store, offlineGlobals, true, fullLoadModels).then(loadingData => Promise.resolve({
        data: loadingData.loadingObjects,
        offlineObjectsData: loadingData.offlineObjectsData
      })).catch(e => Promise.resolve({ data: [] }));
  },

  getDictsForUpdateFromServer(store, offlineGlobals, modelsForCheck, excludedModels, onlyTodayNotSyncing) {
    let self = this,
        modelNameConverter = this.get('modelNameConverter');
    return self.getLoadingObjects(store, offlineGlobals).then(loadingData => offlineGlobals.checkBackendAvailable().then(isOnline => new Promise(function(resolve, reject) {
          if (isOnline) {
            let curDate = new Date();
            if (!isBlank(onlyTodayNotSyncing)) {
              loadingData.loadingObjects = loadingData.loadingObjects.filter(function(item) {
                let loadDate = item.get('lastSuccessLoadDate');
                return isBlank(loadDate) || loadDate.toDateString() !== curDate.toDateString();
              });
            }

            if (!isBlank(modelsForCheck) && modelsForCheck.length > 0) {
              loadingData.loadingObjects = loadingData.loadingObjects.filter(function(item) {
                return modelsForCheck.some(function(modelName) {
                  return modelName === item.modelName;
                });
              });
            }

            if (!isBlank(excludedModels) && excludedModels.length > 0) {
              loadingData.loadingObjects = loadingData.loadingObjects.filter(function(item) {
                return excludedModels.every(function(modelName) {
                  return modelName !== item.modelName;
                });
              });
            }

            let token = self.get('session.data.authenticated.token') || self.get('session.data.authenticated.access_token'),
              checkingDicts = loadingData.loadingObjects.map(function(item) {
                let name = modelClassForType(store, item.modelName),
                  date = item.get('loadDate'),
                  editDate = item.get('editDate'),
                  isFirstLoadCompleted = item.get('isFirstLoadCompleted'),
                  limit = item.get('predicate'),
                  masters = item.get('masters');
                return {
                  modelName: name,
                  dateStart: isBlank(date) ? '' : new Moment(date).subtract(new Moment(date).utcOffset(), 'm').format(config.moment.isoDateTimeWithMsFormat),
                  editDateStart: isBlank(date) || isBlank(editDate) ? '' : new Moment(editDate).subtract(new Moment(editDate).utcOffset(), 'm').format(config.moment.isoDateTimeWithMsFormat),
                  isFirstLoadCompleted: isFirstLoadCompleted,
                  limit: isNone(limit) ? '' : self.serializePredicate(limit),
                  mastersForCheck: isBlank(masters) ? null : masters
                };
              }),
              message = 'Ошибка при загрузке с сервера информации по обновляемым объектам.';
            if (checkingDicts.length > 0) {
              $.ajax({
                url: config.APP.backendUrl + '/api/UpdatingObjects/GetForUpdate',
                data: JSON.stringify({ dateStart: null, dicts: checkingDicts }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                type: 'POST',
                headers: { Authorization: 'Bearer ' + token },
                success: function(data) {
                  if (data) {
                    if (data.error) {
                      resolve({ data: [], error: message, offlineObjectsData: loadingData.offlineObjectsData });
                    } else {
                      let dateEnd = new Moment(data.dateEnd).toDate(),
                        modelsForLoad = data.types.map(function(item) {
                          return {
                            modelName: modelNameConverter
                              .modelNameByClass(item.type),
                            createdCount: item.createdCount,
                            editedCount: item.editedCount
                          };
                        }),
                        dictionaries = loadingData.loadingObjects.filter(function(item) {
                          let models = modelsForLoad.filter(function(x) {
                            return x.modelName === item.get('modelName');
                          });
                          if (models.length > 0) {
                            item.set('createdCount', models[0].createdCount);
                            item.set('editedCount', models[0].editedCount);
                            return true;
                          } 
                            let offlineLoadObjs = loadingData.offlineObjectsData.filterBy('objectsModelName', item.get('modelName'));
                            offlineLoadObjs.forEach(function(offlModel) {
                              offlModel.set('objectsLastSuccessLoadDate', dateEnd);
                              offlModel.save();
                            });
                            return false;
                          
                        });
                      resolve({
                        data: dictionaries,
                        dateEnd: dateEnd,
                        offlineObjectsData: loadingData.offlineObjectsData
                      });
                    }
                  } else {
                    resolve({ data: [], error: message, offlineObjectsData: loadingData.offlineObjectsData });
                  }
                },
                error: function(e) {
                  console.error(message, e);
                  resolve({ error: message, offlineObjectsData: loadingData.offlineObjectsData });
                }
              });
            } else {
              resolve({ data: [], offlineObjectsData: loadingData.offlineObjectsData });
            }
          } else {
            resolve({ data: [], offlineObjectsData: loadingData.offlineObjectsData });
          }
        })).catch(e => 
          Promise.resolve({ data: [], offlineObjectsData: loadingData.offlineObjectsData })))
        .catch(e => 
          Promise.resolve({ data: [] }));
  },

  getOfflineLoadObjects(store) {
    let queryBuilder = new QueryBuilder(store).from('offline-load-object').selectByProjection('OfflineLoadObjectL');
    return store.query('offline-load-object', { useOnlineStore: false });
  },

  checkLoadedPermissions(store) {
    let login = this.get('session.data.authenticated.userName');
    return store.query('offline-permission', { login: login, useOnlineStore: false }).then(results => results.get('length') > 0);
  },

  loadBackupPartData(modelName, flObjectData, needReturnObjs, controller, idPath = 'guid') {
    return new Promise(function(resolve, reject) {
      let store = controller.store,
          dexieService = controller.dexie,
          dexieDB = dexieService.dexie('tmobis', store),
          token = controller.get('session.data.authenticated.token') || controller.get('session.data.authenticated.access_token');
      console.debug(`Запрос на загрузку части ${flObjectData.part + 1} бэкапа для${isNone(flObjectData.netDistrictId) ? '' : ' сет. р-на ' + flObjectData.netDistrictId} модели ${modelName}.`);
      $.ajax({
        url: config.APP.backendUrl + '/api/UpdatingObjects/GetJsonForOffline',
        data: JSON.stringify(flObjectData),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: 'POST',
        headers: { Authorization: 'Bearer ' + token }
      }).done(function(result) {
        if (result.error) {
          console.debug('Ошибка при получении бэкапа для ' + modelName + '.');
          reject();
        }

        if (result.notExists) {
          console.debug('Бэкап для ' + modelName + ' не найден.');
          reject();
        } else {
          const partStr = `Часть ${result.curPart + 1}/${result.partsCount} бэкапа для ${modelName}`;
          console.debug(partStr + " получена, начало обработки.");
          let typeName = modelClassForType(store, modelName),
              objArray = [].concat.apply(
                [],
                JSON.parse(result.data)[typeName].map(x => JSON.parse(x))),
              dateEnd = result.dateEnd;
          objArray.map(record => {
            if (idPath) {
              record.id = record.id[idPath];
            }

            for (let key in record) {
              if (record[key] && record[key].id) {
                record[key] = idPath ? record[key].id[idPath] : record[key].id;
              }

              if (record[key] instanceof Array) {
                for (let i = 0; i < record[key].length; i++) {
                  if (record[key][i].id) {
                    record[key][i] = idPath ? record[key][i].id[idPath] : record[key][i].id;
                  }
                }
              }
            }
          });

          console.debug(partStr + ' обработана, начало записи в БД.');
          try {
            const table = dexieDB.table(modelName);
            table.bulkPut(objArray).then(() => {
              resolve({dateEnd: dateEnd, objArray: (needReturnObjs ? objArray : null), curPart: result.curPart, partsCount: result.partsCount});
              console.debug(partStr + '. Запись в БД завершена.');
            }).catch(e => {
              console.debug(partStr + '. Запись в БД НЕ завершена.' + e);
              try {
                console.debug(JSON.stringify(e));
              } catch(ex){}
              reject(e);
            });
          } catch(e) {
            console.error(partStr + '. Запись в БД НЕ завершена.', e);
            reject(e);
          }
        }

      }).fail(function(error) {
        return reject({modelName: modelName, networkError: true, ex: error});
      });
    });
  },

  /**
   * Метод, релаизующий первоначальную загрузку указанного справочника.
   * Перед загрузкой очищает таблицу в IndexedDB.
   *
   * Текущий контроллер
   * @param { Ember.Controller } controller
   *
   * Массив данных о сущностях, загруженных оффлайн (offline-load-object)
   * @param { Array | offline-load-object } offlineObjectsData
   *
   * JSON-объект справочника
   * @param { Object } dictionary
   *
   * Путь к id у сущности
   * @param { String } idPath = 'guid'
   */
  firstLoadOfflineObjects(controller, offlineObjectsData, dictionary) {
    let self = this,
        localStore = getOwner(controller).lookup('store:local'),
        modelName = dictionary.get('modelName'),
        localAdapter = localStore.adapterFor(modelName);
    return localAdapter.clear(modelName).then(() => new Promise(function(resolve, reject) {
        let store = controller.store,
          netDistrictIds = dictionary.get('netDistrictIds') || [],
          flObjectData = {
            modelName: modelClassForType(store, modelName),
            townId: self.get('session.data.authenticated.town'),
            limitByTown: !isNone(dictionary.get('townPaths')),
            netDistrictId: netDistrictIds.length ? netDistrictIds[0] : null,
            part: 0
          },
          complete = function(dateEnd, objs) {
            try {
              let postSync = dictionary.get('postSync'),
                saveOfflineDateEnd = function() {
                  let model = undefined,
                    models = offlineObjectsData.filterBy('objectsModelName', modelName),
                    count = models.get('length'),
                    resDate = new Moment(dateEnd).toDate();
                  if (count > 0) {
                    model = models.get('firstObject');
                    model.set('objectsLoadDate', resDate);
                    model.set('objectsLoadByEditDate', resDate);
                    model.set('objectsLastSuccessLoadDate', resDate);
                    model.set('objectsIsLoaded', true);

                    while (count > 1) {
                      count--;
                      models.get('lastObject').destroyRecord();
                    }
                  } else {
                    model = store.createRecord('offline-load-object', {
                      objectsModelName: modelName,
                      objectsLoadDate: resDate,
                      objectsLoadByEditDate: resDate,
                      objectsLastSuccessLoadDate: resDate,
                      objectsIsLoaded: true
                    }, false);
                  }

                  return model === undefined ? resolve() : model.save().then(y => resolve()).catch(e => reject());
                };

              if (isNone(postSync)) {
                return saveOfflineDateEnd();
              } 
                return postSync(controller, controller.store, A(objs)).then(() => saveOfflineDateEnd()).catch(() => reject());
              
            } catch (ex) {
              return reject(ex);
            }
          },
          needReturnObjs = !isNone(dictionary.get('postSync')),
          totalObjs = [],
          loadNextBackupPartData = function(x) {
            if (needReturnObjs && x.objArray) {
              totalObjs.push(x.objArray);
            }

            if (x.isFirst || x.curPart + 1 < x.partsCount) {
              if (!x.isFirst) {
                flObjectData.part++;
              }
            } else {
              if (!flObjectData.netDistrictId || netDistrictIds[netDistrictIds.length - 1] === flObjectData.netDistrictId) {
                return complete(x.dateEnd, totalObjs);
              } 
                flObjectData.netDistrictId = netDistrictIds[netDistrictIds.indexOf(flObjectData.netDistrictId) + 1];
                flObjectData.part = 0;
              
            }

            return self.loadBackupPartData(modelName, flObjectData, needReturnObjs, controller).then(loadNextBackupPartData);
          };

        return loadNextBackupPartData({isFirst: true}).catch(ex => {
          console.error('Ошибка при загрузке бэкапа. Очистка таблицы ' + modelName + '.', ex);
          return localAdapter.clear(modelName).then(function() {
            console.debug('Очистка таблицы ' + modelName + ' завершена.');
            reject(ex);
          }).catch((iex) => {
            console.error('Ошибка при очистке таблицы ' + modelName + '.', iex);
            reject(ex);
          });
        });
      }));
  },

  syncDownRecords: function(offlineObjectsData, dictionary, endDate, offlineGlobals, changeTotalProgressCallBack) {
    let self = this,
      onOnlineError = function(ex) {
        dictionary.set('error', 'Сервер не доступен');
        dictionary.set('networkError', true);
        if (changeTotalProgressCallBack) {
          changeTotalProgressCallBack();
        }

        return Promise.resolve(false);
      };

    return offlineGlobals.checkBackendAvailable().then(isOnline => {
      if (isOnline) {
        let store = this.store,
          modelName = dictionary.get('modelName'),
          projectionName = dictionary.get('projection'),
          predicate = dictionary.get('predicate'),
          masters = dictionary.get('masters'),
          loadCount = dictionary.get('loadCount') || 50,
          createdCount = dictionary.get('createdCount'),
          editedCount = dictionary.get('editedCount'),
          postSync = dictionary.get('postSync'),
          isFirstLoadCompleted = dictionary.get('isFirstLoadCompleted'),
          loadDate = dictionary.get('loadDate'),
          loadDateString = isBlank(loadDate) ? null : new Moment(loadDate).format(config.moment.isoForLimitDateTimeWithMsFormat),
          editDate = dictionary.get('editDate'),
          editDateString = isBlank(editDate) ? null : new Moment(editDate).format(config.moment.isoForLimitDateTimeWithMsFormat),
          endDateString = isBlank(endDate) ? null : new Moment(endDate).format(config.moment.isoForLimitDateTimeWithMsFormat),
          localStore = getOwner(this).lookup('store:local'),
          localAdapter = localStore.adapterFor(modelName),
          lastDictDate = undefined,
          getMasterPredicate = function(master, dStartStr, dEndStr) {
            if (dStartStr) {
              return new ComplexPredicate(Condition.Or, ...[
                new ComplexPredicate(Condition.And, ...[
                  new DatePredicate(master + '.createTime', FilterOperator.Ge, dStartStr),
                  new DatePredicate(master + '.createTime', FilterOperator.Leq, dEndStr),
                ]),
                new ComplexPredicate(Condition.And, ...[
                  new DatePredicate(master + '.editTime', FilterOperator.Ge, dStartStr),
                  new DatePredicate(master + '.editTime', FilterOperator.Leq, dEndStr),
                ])
              ]);
            } 
              return new ComplexPredicate(Condition.Or, ...[
                new DatePredicate(master + '.createTime', FilterOperator.Leq, dEndStr),
                new DatePredicate(master + '.editTime', FilterOperator.Leq, dEndStr)
              ]);
            
          },

          addMastersPredicates = function(masters, predicate, dStartStr, dEndStr) {
            let result = [predicate];
            masters.forEach(function(item) {
              result.push(getMasterPredicate(item, dStartStr, dEndStr));
            }, this);

            return new ComplexPredicate(Condition.Or, ...result);
          },

          completeFunc = function(isSuccess, operationNumb, needSaveEditDate) {
            let model = undefined,
              models = offlineObjectsData.filterBy('objectsModelName', modelName),
              count = models.get('length'),
              resDate = isSuccess ? endDate : lastDictDate;
            if (count > 0) {
              model = models.get('firstObject');
              if (operationNumb === 0) {
                if (resDate) {
                  model.set('objectsLoadDate', resDate);
                }

                if (needSaveEditDate && isSuccess) {
                  model.set('objectsLoadByEditDate', resDate);
                  model.set('objectsIsLoaded', true);
                }
              } else {
                if (resDate) {
                  model.set('objectsLoadByEditDate', resDate);
                }

                if (isSuccess && !model.get('objectsIsLoaded')) {
                  model.set('objectsIsLoaded', true);
                }
              }

              model.set('objectsLastSuccessLoadDate', resDate);
              while (count > 1) {
                count--;
                models.get('lastObject').destroyRecord();
              }
            } else {
              model = store.createRecord('offline-load-object', {
                objectsModelName: modelName,
                objectsIsLoaded: isSuccess
              }, false);
              if (isSuccess) {
                if (operationNumb === 0) {
                  model.set('objectsLoadDate', resDate);
                  if (needSaveEditDate) {
                    model.set('objectsLoadByEditDate', resDate);
                  }
                } else {
                  model.set('objectsLoadByEditDate', resDate);
                }

                model.set('objectsLastSuccessLoadDate', resDate);
              }
            }

            return model === undefined ? Promise.resolve(false) : model.save().then(y => isSuccess).catch(e => false);
          },

          syncDownRecordsWC = function(operation, saveEditDate) {
            let queryBuilder = new QueryBuilder(store).from(modelName).selectByProjection(projectionName).count().orderBy(operation === 0 ? 'createTime' : 'editTime'),
              predicates = [];
            if (operation === 0) {
              if (isBlank(loadDate)) {
                predicates.push(new DatePredicate('createTime', FilterOperator.Leq, endDateString));
              } else {
                predicates.push(new DatePredicate('createTime', FilterOperator.Ge, loadDateString));
                predicates.push(new DatePredicate('createTime', FilterOperator.Leq, endDateString));
                if (isFirstLoadCompleted) {
                  predicates.push(new ComplexPredicate(Condition.Or, ...[
                    new SimplePredicate('editTime', FilterOperator.Eq, null),
                    new DatePredicate('editTime', FilterOperator.Ge, endDateString)
                  ]));
                }
              }
            } else {
              let needMastersAdd = masters && masters.length > 0;
              if (isBlank(editDate)) {
                if (!isFirstLoadCompleted && !isBlank(loadDate)) {
                  predicates.push(new DatePredicate('editTime', FilterOperator.Ge, loadDateString));
                  predicates.push(new DatePredicate('editTime', FilterOperator.Leq, endDateString));
                  predicates.push(new DatePredicate('createTime', FilterOperator.Leq, loadDateString));
                } else {
                  predicates.push(new DatePredicate('editTime', FilterOperator.Leq, endDateString));
                  needMastersAdd = false;
                }
              } else {
                predicates.push(new DatePredicate('editTime', FilterOperator.Ge, editDateString));
                predicates.push(new DatePredicate('editTime', FilterOperator.Leq, endDateString));
              }

              if (needMastersAdd) {
                let minDate = isBlank(editDate) || isBlank(loadDate) ? '' : new Moment(editDate).format(config.moment.isoForLimitDateTimeWithMsFormat),
                  wM = addMastersPredicates(
                    masters,
                    new ComplexPredicate(Condition.And, ...predicates),
                    minDate,
                    endDateString
                  );
                predicates = [wM];
              }
            }

            if (predicate) {
              predicates.push(predicate);
            }

            queryBuilder = queryBuilder.where(predicates.length === 1 ? predicates[0] : new ComplexPredicate(Condition.And, ...predicates));
            let syncDownRange = function(position, number) {
              let query = queryBuilder.skip(position).top(number).build();
              query.useOnlineStore = true;
              return store.query(modelName, query).then(function(result) {
                position += number;
                let totalRecordsCount = result.get('meta.count'),
                  localTotalProgress = isNone(postSync) ? 100 : 50,
                  progress = dictionary.get('progress'),
                  curProgress = result.get('length') * localTotalProgress / (createdCount + editedCount),
                  lastObject = result.get('lastObject'),
                  afterPostSync = function() {
                    lastDictDate = isNone(lastObject) ? null : lastObject.get(operation === 0 ? 'createTime' : 'editTime');
                    dictionary.set('progress', progress + curProgress > 100 ? 100 : progress + curProgress);
                    if (changeTotalProgressCallBack) {
                      changeTotalProgressCallBack();
                    }

                    if (position < totalRecordsCount) {
                      return syncDownRange(position, number);
                    }

                    return completeFunc(true, operation, saveEditDate);
                  };

                if (!isNone(postSync)) {
                  let changeLocalProgressWithPostSync = function(postSyncProgress) {
                    let newProgress = progress + curProgress + (curProgress * postSyncProgress / 100);
                    dictionary.set('progress', newProgress > 100 ? 100 : newProgress);
                    if (changeTotalProgressCallBack) {
                      changeTotalProgressCallBack();
                    }
                  };

                  return postSync(self, store, result, changeLocalProgressWithPostSync).then(x => afterPostSync());
                } 
                  return afterPostSync();
                
              }).catch(e => {
                console.error('Ошибка при синхронизации справочника ' + modelName + ' на позиции ' + position + ', загружаемое кол-во ' + number + ': ' + e);
                dictionary.set('error', '' + e);
                if (changeTotalProgressCallBack) {
                  changeTotalProgressCallBack();
                }

                return completeFunc(false, operation, saveEditDate);
              });
            };

            // Загружаем по loadCount записей за один запрос, начиная с 0 позиции.
            return syncDownRange(0, loadCount);
          };

        if (isBlank(loadDate)) {
          return localAdapter.clear(modelName).then(x => syncDownRecordsWC(0, true));
        } 
          if (createdCount > 0) {
            return syncDownRecordsWC(0).then(x => {
              if (x) {
                lastDictDate = undefined;
                return syncDownRecordsWC(1);
              } 
                return Promise.resolve(false);
              
            });
          } 
            return syncDownRecordsWC(1);
          
        
      } 
      return onOnlineError(new Error('Нет подключения к сети'));
    }).catch(ex => onOnlineError(ex));
  },

  getDictsForFullLoadOrSync(store, offlineGlobals, fullLoadModels, checkBigLoadObjs, onlyTodayNotSyncing) {
    let self = this,
        checkBigLoadFunc = function(x) {
          return self.getDictsForUpdateFromServer(store, offlineGlobals, undefined, x.data.map(function(item) {
            return item.modelName;
          }), onlyTodayNotSyncing).then(y => {
            if (y && y.data && y.data.get('length') > 0) {
              y.data.forEach(function(syncModel) {
                let updateObjsCount = syncModel.get('createdCount') + syncModel.get('editedCount');
                if (syncModel.get('loadCount') * 5 < updateObjsCount) {
                  x.data.pushObject(syncModel);
                }
              });
            }

            return x.data && x.data.get('length') > 0 ?
              Promise.resolve({ needFullLoad: true, data: x.data, offlineObjectsData: x.offlineObjectsData }) :
              Promise.resolve(y);
        });
      };
    return self.getDictsForFullLoad(store, offlineGlobals, fullLoadModels).then(x => {
      if (x.data.get('length') > 0) {
        if (checkBigLoadObjs) {
          return checkBigLoadFunc(x);
        } 
          return Promise.resolve({ needFullLoad: true, data: x.data, offlineObjectsData: x.offlineObjectsData });
        
      } 
        if (checkBigLoadFunc) {
          return checkBigLoadFunc(x);
        } 
          return self.getDictsForUpdateFromServer(store, offlineGlobals, undefined, undefined, onlyTodayNotSyncing);
        
      
    });
  },

  cyclicObjsLoading(data, progressProp, func) {
    let self = this,
      index = -1,
      length = data.get('length'),
      changeTotalProgress = function() {
        let sum = 0;
        data.forEach(function(item) { sum += isBlank(item.get('error')) ? item.get('progress') : 100; });
        self.set(progressProp, Math.floor(sum / length));
      },
      syncDownNextDictionary = function () {
        index++;
        if (index === length) {
          return Promise.resolve(null);
        }

        let curDict = data.objectAt(index);
        return func(curDict, changeTotalProgress).then(syncDownNextDictionary);
      };
    return syncDownNextDictionary();
  },

  syncObjectsFunc(loadingData, offlineGlobals) {
    let self = this;
    self.set('loadingDictionaries', loadingData.data);
    self.set('loadingDictionariesTotalProgress', 0);
    self.set('loadingDictionariesVisible', true);
    return self.cyclicObjsLoading(loadingData.data, 'loadingDictionariesTotalProgress', function(curDict, changeTotalProgress) {
      return self.syncDownRecords(loadingData.offlineObjectsData, curDict, loadingData.dateEnd, offlineGlobals, changeTotalProgress);
    }).then(() => {
      self.set('syncObjsErrors', loadingData.data.filter(function(item) { return !isBlank(item.get('error')); }).map(function(item) { return item.get('caption'); }).join(', '));
      self.set('syncNetworkError', loadingData.data.some(m => m.get('networkError')));
      return Promise.resolve();
    });
  },

  startFullLoadAndSync(store, loadingData, offlineGlobals, onlyLoadingDataSync) {
    let self = this;
    if (loadingData.needFullLoad) {
      self.set('loadingBackups', loadingData.data);
      self.set('loadingBackupsTotalProgress', 0);
      self.set('loadingBackupsVisible', true);
      return self.cyclicObjsLoading(loadingData.data, 'loadingBackupsTotalProgress', function(curDict, changeTotalProgress) {
        return self.firstLoadOfflineObjects(self, loadingData.offlineObjectsData, curDict).then(() => {
          curDict.set('progress', 100);
          changeTotalProgress();
          return Promise.resolve();
        }).catch(ex => {
          const isExObj = typeof ex === 'object',
                exMsg = isExObj ? ex.ex : ex;
          curDict.set('error', exMsg + '');
          changeTotalProgress();
          if (isExObj && ex.networkError) {
            curDict.set('networkError', true);
          }
            return Promise.resolve();
        });
      }).then(() => {
        let modelsWithError = loadingData.data.filter(function(item) { return !isBlank(item.get('error')); }),
          modelsForExclude = onlyLoadingDataSync ? undefined : modelsWithError.map(function(item) { return item.get('modelName'); }),
          modelsForLoad = onlyLoadingDataSync ? loadingData.data.map(function(item) { return item.get('modelName'); }) : undefined;
        self.set('fullLoadErrors', modelsWithError.map(function(item) { return item.get('caption'); }).join(', '));
        self.set('fullLoadNetworkError', modelsWithError.some(m => m.get('networkError')));
        return self.getDictsForUpdateFromServer(store, offlineGlobals, modelsForLoad, modelsForExclude).then(newData => {
          if (newData.data.get('length') > 0) {
            return self.syncObjectsFunc(newData, offlineGlobals);
          } 
            return Promise.resolve();
          
        });
      });
    } 
      return self.syncObjectsFunc(loadingData, offlineGlobals);
    
  },

  clearInterfaceProps() {
    this.set('loadingDictionariesVisible', false);
    this.set('loadingBackupsVisible', false);
    this.set('loadingDictionariesTotalProgress', 0);
    this.set('loadingDictionaries', undefined);
    this.set('loadingBackupsTotalProgress', 0);
    this.set('loadingBackups', undefined);
    this.set('loadErrors', undefined);
    this.set('fullLoadErrors', undefined);
    this.set('syncObjsErrors', undefined);
  }
});
