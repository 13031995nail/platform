import { Promise } from 'rsvp';
import config from 'ember-get-config';
import { computed, observer } from '@ember/object';
import { getOwner } from '@ember/application';
import { inject as service } from '@ember/service';
import BaseStore from 'ember-flexberry-data/stores/base-store';
import QueryObject from 'ember-flexberry-data/query/query-object';
import OnlineStore from '../stores/online-store';

/**
 * @description комбинированное хранилище данных онлайн/офлайн
 */
export default BaseStore.extend({
  network: service(),
  offlineState: service('offline-globals'),
  offlineEntities: service('offline/offline-entities'),
  offlineSchemaProvider: service('offline/offline-schema'),

  init() {
    const offlineSchema = this.get('offlineSchemaProvider.offlineSchema');
    this.set('offlineSchema', offlineSchema);
    
    this._super(...arguments);

    const owner = getOwner(this),
          onlineStore = OnlineStore.create(owner.ownerInjection());
    this.set('onlineStore', onlineStore);
    this.set('offlineStore.dbName', config.APP.offline.dbName);
    this.offlineModels = this.offlineModelsNew;
  },

  offlineModelsNew: computed('offlineState.backupSynced','offlineState.offlineModeAvailable',
    'offlineEntities.backupEntities', function () {
    let models = {
      'application-cache': true,
      'offline-permission': true,
      'offline-user': true,
      'offline-load-object': true,
      'offline-app-setting': true,
      'print-service': true,
      'print-service-printer': true,
      'change-password': true,
      'new-platform-flexberry-flexberry-user-setting': true,
      'i-c-s-soft-s-t-o-r-m-n-e-t-business-audit-objects-audit-entity': true,
      'i-c-s-soft-s-t-o-r-m-n-e-t-business-audit-objects-audit-field': true,
      'i-c-s-soft-s-t-o-r-m-n-e-t-business-audit-objects-object-type': true,
      'i-c-s-soft-s-t-o-r-m-n-e-t-security-agent': true,
      'i-c-s-soft-s-t-o-r-m-n-e-t-security-link-group': true,
      'i-c-s-soft-s-t-o-r-m-n-e-t-security-session': true,
    };

    const offlineState = this.get('offlineState'),
          offlineReady = offlineState.get('offlineModeAvailable')
            && offlineState.get('backupSynced'),
          backupEntities = this.get('offlineEntities.backupEntities');

    backupEntities.forEach(entity => {
      models[entity.modelName] = offlineReady;
    });

    return models;
  }),

  offlineModelsObserver: observer('offlineModelsNew', function () {
    this.set('offlineModels', this.get('offlineModelsNew'));
  }),

  catchNetworkError(queryPromise, superInvoker) {
    let self = this,
      repeateQuery = function (newQuery, switchingToOffline) {
        let thisRouter = getOwner(self).lookup('controller:application');
        if (newQuery) {
          try {
            queryPromise = superInvoker(switchingToOffline);
          } catch (e) { }
        }

        return queryPromise.catch(function (e) {
          console.error('Ошибка при выполнении запроса к БД', e);
          if (typeof e === 'object' && e !== null && e.message === "The adapter operation timed out") {
            return thisRouter.initAndShowMDAlertPromise('Загрузка данных прервана из-за истечения времени ожидания результатов.').then(() => Promise.reject(e));
          }
          return self.get('network').checkNetwork().then(connectionSuccess => {
            if (connectionSuccess) {
              if (typeof e === 'object' && e !== null && e.unauthorizedException) {
                return Promise.reject(e);
              }
              let errorMessage;
              if (e.name === "VersionError") {
                errorMessage = "Ошибка несоответствия версии локальной базы данных. Очистите локальную базу данных, для этого перейдите в Настройки -> Удалить локальную базу данных."
              }
              else {
                errorMessage = "Ошибка при выполнении запроса к базе данных. Соединение с центральной базой подтверждено. Повторить попытку?";
              }
              return thisRouter.showMdConfirmation(errorMessage).then(x => {
                if (x) {
                  return repeateQuery(true);
                }
                return Promise.reject(e);

              });

            }

            return Promise.reject(new Error('Пропало соединение с центральной базой'));

            // TODO: Убрать в случае ненадобности
            // return thisRouter.showMdConfirmation('Пропало соединение с центральной базой. Повторить попытку?').then(x => {
            //   if (x) {
            //     return repeateQuery(true);
            //   }
            //   if (self.get('offlineState.offlineModeAvailable')) {
            //     self.set('offlineState.isOnline', false);
            //     return thisRouter.showMdAlert('Приложение работает в оффлайн-режиме.').then(y => repeateQuery(true, true));
            //   }
            //   return thisRouter.showMdAlert('Приложение не может быть переключено в оффлайн-режим, так как включен режим постоянного онлайна. Выполняется повторная попытка.').then(y => repeateQuery(true));
            // });

          });
        });
      };

    return repeateQuery();
  },

  logOfflineError(e) {
    console.error('Ошибка при выполнении запроса к локальной БД: ' + e.message, e);
    return e;
  },

  findAll: function (modelName, options) {
    let self = this,
      sup = this._super,
      args = arguments,
      superInvoker = function (switchingToOffline) {
        if (switchingToOffline && args[1] instanceof QueryObject) {
          self.changeQueryObjectToOffline(args[1]);
        }

        return sup.apply(self, args);
      };

    let result = superInvoker();

    // В следующих случаях не обрабатываем ошибку сети:
    // 1. Изначально в параметрах запроса задано использование локальной БД.
    // 2. В параметрах запроса задано использование удаленной БД: такой запрос нельзя перенаправить в ЛБД.
    // 3. Текущий режим работы приложения - оффлайн.
    if ((options && options.useOnlineStore === false) ||
      (options && options.useOnlineStore === true) ||
      (!this.get('offlineState.isOnline'))) {
      return result.catch(e => self.logOfflineError(e));
    }

    return this.catchNetworkError(result, superInvoker);
  },

  findRecord: function (modelName, id, options) {
    let self = this,
      sup = this._super,
      args = arguments,
      superInvoker = function (switchingToOffline) {
        if (switchingToOffline && args[1] instanceof QueryObject) {
          self.changeQueryObjectToOffline(args[1]);
        }

        return sup.apply(self, args);
      };

    let result = superInvoker();

    // В следующих случаях не обрабатываем ошибку сети:
    // 1. Изначально в параметрах запроса задано использование локальной БД.
    // 2. В параметрах запроса задано использование удаленной БД: такой запрос нельзя перенаправить в ЛБД.
    // 3. Текущий режим работы приложения - оффлайн.
    if ((options && options.useOnlineStore === false) ||
      (options && options.useOnlineStore === true) ||
      (!this.get('offlineState.isOnline'))) {
      return result.catch(e => self.logOfflineError(e));
    }

    return this.catchNetworkError(result, superInvoker);
  },

  query: function (modelName, query) {
    let self = this,
      sup = this._super,
      args = arguments,
      useOnlineStore = query.useOnlineStore,
      superInvoker = function (switchingToOffline) {
        if (switchingToOffline && args[1] instanceof QueryObject) {
          self.changeQueryObjectToOffline(args[1]);
        }

        return sup.apply(self, args);
      };

    let result = superInvoker();

    // В следующих случаях не обрабатываем ошибку сети:
    // 1. Изначально в параметрах запроса задано использование локальной БД.
    // 2. В параметрах запроса задано использование удаленной БД: такой запрос нельзя перенаправить в ЛБД.
    // 3. Текущий режим работы приложения - оффлайн.
    if ((query && useOnlineStore === false) ||
      (query && useOnlineStore === true) ||
      (!this.get('offlineState.isOnline'))) {
      return result.catch(e => self.logOfflineError(e));
    }

    return this.catchNetworkError(result, superInvoker);
  },

  // TODO: Обходной маневр для починки queryRecord из-за
  // технологической проблемы с jQuery сериализацией predicate.and метода
  // в составе запроса
  queryRecord: function (modelName, query) {
    return this.query(modelName, query)
      .then((result) => result ? result.get('firstObject') : undefined);
  },

  changeQueryObjectToOffline(obj) {
    let props = Object.keys(obj);
    if (props.indexOf('primaryKeyName') !== -1) {
      obj.primaryKeyName = 'id';
    }

    if (props.indexOf('expand') !== -1) {
      let expandProps = Object.keys(obj.expand),
        self = this;
      expandProps.forEach(function (element) {
        self.changeQueryObjectToOffline(obj.expand[element]);
      }, this);
    }

    if (props.indexOf('extend') !== -1) {
      this.changeQueryObjectToOffline(obj.extend);
    }
  }
});
