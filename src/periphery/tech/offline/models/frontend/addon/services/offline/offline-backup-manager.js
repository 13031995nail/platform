import Service, { inject as service } from '@ember/service';
import { computed } from '@ember/object';
import { isBlank } from '@ember/utils';
import { getOwner } from '@ember/application';
import config from 'ember-get-config';
import { later } from '@ember/runloop';
import moment from 'moment';
import PredicatSerializer from '../../serializers/predicate-serializer';

export default Service.extend(PredicatSerializer, {
  i18n: service(),
  store: service(),
  modals: service(),
  network: service(),
  session: service(),
  dexie: service(),
  file: service(),
  appController: undefined,
  httpService: service('http-request'),
  offlineState: service('offline-globals'),
  offlineError: service('offline/offline-error'),
  modelNameConverter: service('model-name-converter'),
  offlineBackupLoader: service('offline/offline-backup-loader'),

  isSycningInProgress: false,
  loadingPermissionVisible: false,
  loadingPermissionProgress: 0,

  connectTimeoutTime: undefined,
  currentMode: undefined,

  init() {
    this._super(...arguments);
    this.set('appController',
      getOwner(this).lookup('controller:application'));
  },

  savedIsOnline: computed('session.data.authenticated.isOnline', function () {
    return !isBlank(this.get('session.data.authenticated.isOnline'))
      ? this.get('session.data.authenticated.isOnline')
      : true;
  }),

  isOfflineEnabled: computed(
    'offlineState.offlineModeAvailable', 'savedIsOnline', function () {
    return this.get('offlineState.offlineModeAvailable') 
      && this.get('savedIsOnline');
  }),

  async onAuthSuccessful() {
    await this.syncFull();
  },

  async syncFull() {
    this.set('currentMode', 'full');
    await this.syncInternal({needDicts: true, needPerms: true});
  },

  async syncDictionaries() {
    this.set('currentMode', 'dicts');
    await this.syncInternal({needDicts: true, needPerms: false});
  },

  async syncPermissions() {
    this.set('currentMode', 'perms');
    await this.syncInternal({needDicts: false, needPerms: true});
  },

  /** @protected */
  async syncInternal(params) {
    const self = this,
          i18n = this.get('i18n'),
          offlineError = this.get('offlineError'),
          isOfflineEnabled = this.get('isOfflineEnabled'),
          isSycningInProgress = this.get('isSycningInProgress');

    if (isOfflineEnabled && !isSycningInProgress) {
      let syncSuccess = false;
      try {
        self.set('isSycningInProgress', true);
        //self.showLoading(params.needDicts, params.needPerms);

        const [loadObjs, loadPerms] = await Promise.all([
          self.checkDictionaries(params.needDicts),
          self.checkPermissions(params.needPerms)
        ]);

        const needLoad = loadObjs.need || loadPerms.need;
        self.switchInterface(loadObjs.need, loadPerms.need);

        if (needLoad) {
          let title = i18n.t('features.offline.loadOffline.switchedTitle'),
              fullMessage = self.getSuccessMessage(
                loadObjs.need, loadPerms.need);

          try {
            const isFreeSpaceAvailable = await self.isFreeSpaceAvailable();
            if (isFreeSpaceAvailable) {
              const [statusDicts, statusPerms] = await Promise.all([
                self.syncDictionariesInternal(loadObjs),
                self.syncPermissionsInternal(loadPerms.need)
              ]);
      
              let statusMessages = [];
              if (statusDicts.error) {
                offlineError.offlineMessage(statusMessages);
              }
      
              statusMessages = statusMessages
                .concat(statusDicts.messages)
                .concat(statusPerms.messages);
      
              const hasErrors = statusDicts.error || statusPerms.error;
              syncSuccess = !hasErrors;
              if (hasErrors) {
                title = i18n.t('features.offline.loadOffline.failedTitle');
                fullMessage = statusMessages.join('\n');
              }

              if (statusDicts.networkError) {
                throw new Error();
              }
            } else {
              title = i18n.t('features.offline.loadOffline.failedTitle');
              fullMessage = i18n.t(
                'features.offline.loadOffline.notEnoughFreeSpace');
            }

            await self.onSyncComplete({ message: fullMessage, title: title });
          } catch (reason) {
            console.log(reason);
            await self.showRetryModal({ message: fullMessage, title: title });
          } finally {
            self.unsetOfflineLoading(loadObjs.need);
          }
        } else {
          syncSuccess = true;
        }
      } catch (ex) {
        console.error('Ошибка при проверке загруженных объектов.', ex);
        self.hideInterface();
      }

      self.set('offlineState.backupSynced', syncSuccess);
      self.set('isSycningInProgress', false);
    }
  },

  async showRetryModal(message) {
    const self = this,
          modals = this.modals;

    try {
      await modals.showConfirmation({
          title: 'Желаете остановить загрузку?',
          message: 'При выгрузке данных в локальное хранилище была потеряна связь с интернетом'});
      
      await self.onSyncComplete(message);
    } catch (_) {
      self.startPeriodicOnlineCheck();
    }
  },

  async exportLocaleDB() {
    const dexieService = this.get('dexie');
    const store = this.get('store');
    const fileService = this.get('file');
    const modals = this.get('modals');
    const dexieDB = dexieService.dexie('tmobis', store);
  
    try {
      modals.showLoading({ message: 'Начало экспорта локальной БД...' });
  
      const exportData = {};
      for (const table of dexieDB.tables) {
        const data = await table.toArray();
        exportData[table.name] = data; 
      }
  
      const payload = {
        version: dexieDB.verno,
        exportedAt: new Date().toISOString(),
        data: exportData
      };
  
      const filename = `dexie-backup-${new Date().toISOString().slice(0, 10)}.json`;
      const blob = new Blob([JSON.stringify(payload)], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      const isRunningOnDevice = config.APP.runningOnDevice;
  
      if (!isRunningOnDevice) {
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        modals.showModal({ title: 'Файл бэкапа успешно создан' });
      } else {
        const fileId = await fileService.addFileByUrl(url, filename);
        await fileService._createExternalFileOnDevice(fileId);
        fileService.forget(fileId);
        modals.showModal({ title: 'Файл бэкапа сохранён на устройстве' });
      }
    } catch (e) {
      console.error('Ошибка экспорта', e);
    } finally {
      await modals.hideLoading(true);
    }
  },

  async getDexieBackupJsonData() {
    const fileService = this.get('file');
    let fileId;
    try {
      fileId = await fileService.addFile('.json');
      const blob = await fileService.loadFileById(fileId);
      if (!blob) {
        throw new Error('Не удалось прочитать выбранный файл.');
      }
      return typeof blob.text === 'function'
        ? await blob.text()
        : await new Promise((resolve, reject) => {
          const reader = new FileReader();
          reader.onload = () => resolve(reader.result);
          reader.onerror = () => reject(new Error('Ошибка чтения файла'));
          reader.readAsText(blob);
        });
    } finally {
      if (fileId) {
        fileService.forget(fileId);
      }
    }
  },

  async importDexieDataFromJsonText(jsonText) {
    const dexieService = this.get('dexie');
    const store = this.get('store');
    const dexieDB = dexieService.dexie('tmobis', store);
    const parsedJson = JSON.parse(jsonText);
    if (parsedJson.version !== dexieDB.verno) {
      throw new Error(
        `Несовпадение версий: файл ${parsedJson.version}, приложение ${dexieDB.verno}`);
    }
    console.debug('Начало импорта данных в локальную БД.');
    for (const [tableName, rows] of Object.entries(parsedJson.data)) {
      const table = dexieDB.table(tableName);
      if (table) {
        await table.bulkPut(rows);
      }
    }
    parsedJson.data = null;
    console.debug('Импорт локальной БД завершён.');
  },

  async syncSessionAfterImport() {
    const offlineState = this.get('offlineState');
    if (!offlineState.get('offlineModeAvailable')) {
      offlineState.set('checkedToggleOnline', false);
      return;
    }
    try {
      if (offlineState.get('isOnline')) {
        await offlineState.toggleOffline();
      }
    } catch (e) {
      console.error(
        'После импорта ЛБД не удалось переключить сессию в офлайн-режим', e);
    } finally {
      offlineState.set('checkedToggleOnline', false);
    }
  },

  async importLocaleDB() {
    const modals = this.get('modals');
    try {
      await modals.showConfirmation({
        message: 'Текущая локальная база данных будет заменена данными из файла. Продолжить?',
      });
    } catch (e) {
      console.error('Импорт отменён пользователем', e);
      return;
    }
    await modals.waitHidden();

    const prepared = await this.offlineBackupLoader.clearDictionariesForReload(this);
    if (!prepared) {
      console.error('Не удалось подготовить локальное хранилище к импорту.');
      await modals.showAlert({
        title: 'Ошибка',
        message: 'Не удалось подготовить локальное хранилище к импорту.',
        alertClass: 'negative',
      });
      return;
    }

    let jsonText;
    try {
      jsonText = await this.getDexieBackupJsonData();
    } catch (e) {
      console.error('Ошибка при выборе файла бэкапа:', e);
      await modals.showAlert({
        title: 'Ошибка',
        message: typeof e === 'string' ? e : (e.message || String(e)),
        alertClass: 'negative',
      });
      return;
    }

    modals.showLoading({ message: 'Импорт локального хранилища...' });
    try {
      await this.importDexieDataFromJsonText(jsonText);
      await this.syncSessionAfterImport();
      await modals.hideLoading(true);
      await modals.showAlert({ message: 'База данных успешно импортирована.' });
    } catch (e) {
      console.error('Ошибка при разборе файла бэкапа:', e);
      await modals.hideLoading(true);
      await modals.showAlert({
        title: 'Ошибка импорта',
        message: e.message || String(e),
        alertClass: 'negative',
      });
    }
  },

  /** @protected */
  async checkDictionaries(need) {
    const self = this,
          store = this.get('store'),
          offlineState = this.get('offlineState'),
          isOfflineEnabled = this.get('isOfflineEnabled');
    let result = { need: false, objs: undefined };
    
    if (isOfflineEnabled && need) {
      result.objs = await self.offlineBackupLoader.getDictsForFullLoadOrSync(
        store, offlineState, null, true);
      result.need = !!result.objs && !!result.objs.data
        && result.objs.data.get('length') > 0;
    }

    return result;
  },

  /** @protected */
  async checkPermissions(need) {
    const self = this,
          store = this.get('store'),
          isOfflineEnabled = this.get('isOfflineEnabled');
    let result = { need: false };
    
    if (isOfflineEnabled && need) {
      // TODO: Нужно ли проверять?
      const perms = await self.offlineBackupLoader.checkLoadedPermissions(store);
      result.need = true;
    }

    return result;
  },

  /** @protected */
  async onSyncComplete(message) {
    const self = this,
          modals = this.get('modals');
    setTimeout(() => {
      // TODO: Modal timeout
      modals.showAlert(message);
    }, 1000);
    self.offlineBackupLoader.clearInterfaceProps();
    self.set('loadingPermissionVisible', false);
    self.set('loadingPermissionProgress', 0);
    self.hideInterface();
  },

  /** @protected */
  switchInterface(needLoadObjs, needLoadPerms) {
    const self = this,
          offlineState = this.get('offlineState'),
          needLoad = needLoadObjs || needLoadPerms;

    if (needLoad) {

      if (needLoadObjs) {
        offlineState.set('isSyncDownWhenOnlineEnabled', false);
        offlineState.set('synchronousSyncDownEnabled', true);
      }

      if (needLoadPerms) {
        self.set('loadingPermissionProgress', 0);
        self.set('loadingPermissionVisible', true);
      }
    } else {
      self.hideInterface();
    }
  },

  /** @protected */
  hideInterface() {
    const self = this,
          appController = this.get('appController'),
          modals = this.get('modals'),
          offlineState = this.get('offlineState'),
          savedIsOnline = this.get('savedIsOnline');

    // modals.hideLoading();
    if (!savedIsOnline) {
      offlineState.setOnlineAvailable(savedIsOnline);
    }
    // self.showSwitchedMessage(savedIsOnline);
  },
  
  /** @protected */
  showSwitchedMessage(savedIsOnline) {
    let i18n = this.get('i18n'),
        modals = this.get('modals'),
        msgTypeKey = savedIsOnline
          ? 'switchedToOnline' : 'switchedToOffline',
        msg = i18n.t(`features.offline.loadOffline.${msgTypeKey}`),
      msgTitle = i18n.t(`features.offline.loadOffline.switchedTitle`);
    modals.showModal({message: msg, title: msgTitle});
  },

  /** @protected */
  unsetOfflineLoading(needLoadObjs) {
    const offlineState = this.get('offlineState');
    if (needLoadObjs) {
      offlineState.set('synchronousSyncDownEnabled', false);
      offlineState.set('isSyncDownWhenOnlineEnabled',
        config.APP.offline.syncDownWhenOnlineEnabled);
    }
  },

  /** @protected */
  async syncDictionariesInternal(loadObjs) {
    const self = this,
          i18n = this.get('i18n'),
          store = this.get('store'),
          offlineState = this.get('offlineState'),
          offlineBackupLoader = this.get('offlineBackupLoader');
    let result = { error: false, messages: [], networkError: false };

    if (loadObjs.need) {
      await self.offlineBackupLoader.startFullLoadAndSync(
        store, loadObjs.objs, offlineState);

      const fullLoadErrors = self.get('offlineBackupLoader.fullLoadErrors'),
            syncObjsErrors = self.get('offlineBackupLoader.syncObjsErrors'),
            fullLoadNetworkError = self.get('offlineBackupLoader.fullLoadNetworkError'),
            syncObjsNetworkError = self.get('offlineBackupLoader.syncObjsNetworkError');

      result.networkError = fullLoadNetworkError || syncObjsNetworkError;
      offlineBackupLoader.set('loadErrors', fullLoadErrors);
      
      if (!isBlank(fullLoadErrors)) {
        const errorMessage =
          i18n.t('features.offline.loadOffline.failedToLoadBackup') +
          ' ' + fullLoadErrors;
        result.messages.push(errorMessage);
        result.error = true;
      }

      if (!isBlank(syncObjsErrors)) {
        const errorMessage =
          i18n.t('features.offline.loadOffline.failedToSyncBackup') +
          ' ' + syncObjsErrors;
        result.messages.push(errorMessage);
        result.error = true;
      }
    }
    
    return result;
  },

  /** @protected */
  async syncPermissionsInternal(needLoadPerm) {
    const self = this;
    let result = { error: false, messages: [] };

    if (needLoadPerm) {
      const errorMessage = await self.loadPermissions();
      if (!isBlank(errorMessage)) {
        result.error = true;
        result.messages.push(errorMessage);
      }
    }

    return result;
  },

  /** @protected */
  async loadPermissions() {
    const self = this,
          store = this.get('store'),
          session = this.get('session'),
          httpService = this.get('httpService'),
          isOnline = this.get('offlineState.isOnline'),
          modelNameConverter = this.get('modelNameConverter'),
          login = session.get('data.authenticated.userName');

    if (isOnline) {
      try {
        const result = await httpService.ajaxAuthorized({
          url: config.APP.backendUrls.security + '/GetPermissions',
          type: 'GET',
          dataType: 'json',
        });
  
        try {
          self.set('loadingPermissionProgress', 50);
          // Удаление старых полномочий текущего пользователя.
          const records = await store.query('offline-permission', 
            { login: login, useOnlineStore: false });
          let sc = 0,
              offs = Math.floor(Object.keys(result).length / 8),
              curProgress = 60;

          self.set('loadingPermissionProgress', curProgress);
          records.forEach(function (item) {
            item.destroyRecord();
          });

          // Создание новых записей полномочий текущего пользователя.
          for (let modelName in result) {
            sc++;
            let objectPermissions = result[modelName],
              model = store.createRecord('offline-permission', {
                login: login,
                object: modelNameConverter.modelNameByClass(modelName),
                actions: objectPermissions.join(',')
              }, false);
            model.save();
            sc++;
            if (sc === offs) {
              curProgress += 5;
              self.set('loadingPermissionProgress', curProgress);
              sc = 0;
            }
          }

          self.set('loadingPermissionProgress', 100);
          return '';
        } catch (ex) {
          const message = 'Ошибка при обработке полученных с сервера полномочий пользователя.';
          console.error(message, e);
          return message;
        }
      } catch (ex) {
        const message = 'Ошибка при загрузке с сервера полномочий пользователя.';
        console.error(message, e);
        return message;
      } 
    } else {
      return 'Невозможно загрузить полномочия пользователя при нахождении в оффлайн-режиме';
    }
  },
  
  /** @protected */
  async reloadDictionaries() {
    await this.offlineBackupLoader.clearDictionariesForReload();
    await this.syncInternal({ needDicts: true, needPerms: false });
  },

  showLoading(needDicts, needPerms) {
    const modals = this.get('modals'),
          loadingText = this.getLoadingMessage(needDicts, needPerms);
    modals.showLoading(loadingText);
  },

  /** @protected */
  getSuccessMessage(needDicts, needPerms) {
    const i18n = this.get('i18n');
    let msgKey = undefined;
    if (needDicts && needPerms) {
      msgKey = 'successFullLoad';
    } else if (needDicts) {
      msgKey = 'successDictsLoad';
    } else if (needPerms) {
      msgKey = 'successPermsLoad';
    }
    return i18n.t(`features.offline.loadOffline.${msgKey}`);
  },

  /** @protected */
  getLoadingMessage(needDicts, needPerms) {
    const i18n = this.get('i18n');
    let msgKey = undefined;
    if (needDicts && needPerms) {
      msgKey = 'checkingFullLoaded';
    } else if (needDicts) {
      msgKey = 'checkingDictsLoaded';
    } else if (needPerms) {
      msgKey = 'checkingPermsLoaded';
    }
    return i18n.t(`features.offline.loadOffline.${msgKey}`);
  },

  /** @protected */
  async isFreeSpaceAvailable() {
    const runningOnDevice = config.APP.runningOnDevice;
    return runningOnDevice
      ? await this.getDeviceFreeMemory() > 680
      : true;
  },

  /** @protected */
  getDeviceFreeMemory() {
    return new Promise((resolve, reject) => {
      try {
        DiskSpacePlugin.info({ location : 2 },
          (result) => {
            const freeMemoryData = result.free / (1024 * 1024);
            resolve(freeMemoryData);
          },
          (error) => {
            reject(error);
          }
        );
      } catch (ex) {
        resolve(9999);
        return;
      }
    });
  },

  startPeriodicOnlineCheck() {
    const timeout = moment().add(2, 'm');
    this.set('connectTimeoutTime', timeout);
    later(this, this.periodicOnlineChecked, config.APP.offline.periodicCheckStep);
  },

  async periodicOnlineChecked() {
    const network = this.network,
          connectTimeoutTime = this.get('connectTimeoutTime');

    const result = await network.checkNetwork();
    if (result) {
      const currentMode = this.get('currentMode');
      switch (currentMode) {
        case 'full':
          this.syncFull();
          break;
        case 'dicts':
          this.syncDictionaries();
          break;
        case 'perms':
          this.syncPermissions();
          break;
        default:
          break;
      }
    } else {
      if (connectTimeoutTime > moment()) {
        later(this, this.periodicOnlineChecked,
          config.APP.offline.periodicCheckStep);
      } else {
        this.showRetryModal();
      }
    }
  },
  
  reloadDict(modelName, controller) {
    let self = this,
      store = this.get('store'),
      offlineState = this.get('offlineState'),
      objectlistviewEvents = controller.get('objectlistviewEvents'),
      offlineBackupLoader = this.get('offlineBackupLoader'),
      olvComponentName = controller.get('objectlistviewComponentName');
    
    if (isBlank(modelName)) {
      return { success: false, message: 'Ошибка при получении данных справочника. Повторите операцию позже.' };
    }
  
    return offlineBackupLoader.getDictsForFullLoadOrSync(store, offlineState, [modelName]).then(results => {
      let needLoadObjs = results.data.get('length') > 0;
      
      if (needLoadObjs) {
        results.data = results.data.filter(function (item) {
          return item.get('modelName') === modelName;
        });
        
        offlineState.set('isSyncDownWhenOnlineEnabled', false);
        offlineState.set('synchronousSyncDownEnabled', true);
        
        return offlineBackupLoader.startFullLoadAndSync(store, results, offlineState, true).then(function (result) {
          let notAllDicLoaded = results.data.any(function (item) { return !Ember.isBlank(item.get('error')); }),
            dictLoaded = results.data.filter((item) => Ember.isBlank(item.get('error'))).map(x => x.caption),
            messages = [],
            fullLoadErrors = offlineBackupLoader.get('fullLoadErrors'),
            syncObjsErrors = offlineBackupLoader.get('syncObjsErrors'),
            loadErr = offlineBackupLoader.get('loadErrors');
            
          if (dictLoaded.length != 0) {
            offlineBackupLoader.set('loadErrors', loadErr.replace(new RegExp('[,\\s]{0,1}' + dictLoaded[0] + '[,\\s]{0,1}', 'g'), ''));
          }
          
          if (!Ember.isBlank(fullLoadErrors) || !Ember.isBlank(syncObjsErrors)) {
            if (!Ember.isBlank(fullLoadErrors)) {
              messages.push('Не удалось загрузить бэкап справочника');
            }
  
            if (!Ember.isBlank(syncObjsErrors)) {
              messages.push('Не удалось синхронизировать справочник');
            }
            
            return { success: false, message: messages.join('. ') };
          } else {
            messages.push('Данные по справочнику обновлены');
            
            if (!isEmpty(olvComponentName) && !isEmpty(objectlistviewEvents)) {
              objectlistviewEvents.refreshListTrigger(olvComponentName);            
            }
            
            offlineBackupLoader.clearInterfaceProps();
            return { success: true, message: messages.join('. ') };
          }
        }).catch(function (reason) {
          console.error(reason);
          offlineBackupLoader.clearInterfaceProps();
          return { success: false, message: 'Ошибка: ' + reason };
        }).finally(function () {
          offlineState.set('synchronousSyncDownEnabled', false);
          offlineState.set('isSyncDownWhenOnlineEnabled', config.APP.offline.syncDownWhenOnlineEnabled);
        });
      } else {
        return { success: true, message: 'Данные по справочнику актуальны на текущий момент' };
      }
    }).catch(function (error) {
      return { success: false, message: 'Ошибка при загрузке данных: ' + error };
    });
  }
});
