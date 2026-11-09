import config from 'ember-get-config';
import { computed } from '@ember/object';
import { inject as service } from '@ember/service';
import { EventSource } from '@timepath/tmobis-core/utils/events';
import OfflineGlobals from 'ember-flexberry-data/services/offline-globals';

/**
 * @description сервис состояния офлайн режима
 */
export default OfflineGlobals.extend({
  store: service(),
  modes: service(),
  syncer: service(),
  session: service(),
  network: service(),
  offlineUser: service('offline/offline-user'),
  switchOffline: service('offline/switch-offline'),

  checkedToggleOnline: true,

  /** 
   * @description Флаг, выполнена ли успешно синхронизация офлайн бэкапа.
   */
  backupSynced: false,
  offlineModeAvailable: false,

  processingQuery: false,
  synchronousSyncDownEnabled: false,

  movedOnline: undefined,
  movedOffline: undefined,

  init() {
    this._super(...arguments);
    this.set('movedOnline', new EventSource());
    this.set('movedOffline', new EventSource());
  },

  async onAppRouteInitialized() {
    await this.setOfflineModeAvailable();
  
    if (!this.offlineModeAvailable) {
      return;
    }
  
    const isOnlineSaved = await this.offlineUser.getUserOnlineState();
  
    if (isOnlineSaved) {
      this.set('checkedToggleOnline', true);
      this.set('isOnlineField', true);
    } else {
      this.set('checkedToggleOnline', false);
      this.set('isOnlineField', false);
      await this.toggleOffline();
    }
  },

  async onAuthSuccessful(options) {
    await this.setOfflineModeAvailable(options);
  },

  /**
   * @protected
   * @description установить доступность офлайна для запуска из приложения
   */
  async setOfflineModeAvailable(options) {
    if (options && ('onlineModeOn' in options)) {
      this.set('offlineModeAvailable', !options.onlineModeOn);
      return;
    }

    const offlineUserProvider = this.offlineUser,
          offlineUser = await offlineUserProvider.getOfflineUser();
    this.set('offlineModeAvailable', this.modes.get('runningOnDeviceApp')
      || (offlineUser && offlineUser.get('offlineModeAvailable')));
  },

  /**
   * @public
   * @override
   * @description проверка досутпности онлайна
   */
  checkOnlineAvailable() {
    if (!navigator || !navigator.connection) {
      return this._super(...arguments);
    }

    try {
      return navigator.connection.type !== window.Connection.NONE;
    } catch (error) {
      return navigator.connection.type !== 'none';
    }
  },

  /**
   * @public
   * @override
   * @returns есть ли связь с бэкендом
   */
  checkBackendAvailable() {
    return this.network.checkNetwork();
  },

  onMovedOnline() {
    this.movedOnline.trigger();
  },

  onMovedOffline() {
    this.movedOffline.trigger();
  },

  /**
   * @protected
   * @description актуальное состояние офлайна
   */
  isOnlineField: true,

  /**
   * @public
   * @description состояние офлайна
   */
  isOnline: computed('isOnlineField', {
    get: function () {
      return this.isOnlineField;
    },
    set: function (_, value) {
      const oldValue = this.isOnlineField;
      this.set('isOnlineField', value);
      this.set('checkedToggleOnline', value);
      if (value !== oldValue) {
        if (value) {
          this.onMovedOnline();
        } else {
          this.onMovedOffline();
        }
      }

      return value;
    }
  }),

  async toggleOffline() {
    await this.toggleMode(false);
  },

  async toggleOnline() {
    await this.toggleMode(true);
  },

  async toggleMode(newMode) {
    const store = this.store,
          session = this.session,
          offlineUserProvider = this.offlineUser,
          isOnline = this.get('isOnline');

    if (isOnline != newMode) {
      this.setOnlineAvailable(newMode);

      const isAuthenticated = session.get('isAuthenticated');
      // Если мы аутентифицированы, то при переходе
      // нужно поменять autentificator.
      if (!isAuthenticated) {
        return;
      }

      const offlineUser = await offlineUserProvider.getOfflineUser();
      if (!offlineUser) {
        return;
      }

      const login = offlineUser.get('userName'),
            pass = offlineUser.get('pwd');
      offlineUser.set('isOnline', newMode);
      offlineUser.save();

      if (!login || !pass) {
        return;
      }

      const authenticatorConfigName = (newMode)
        ? 'ember-simple-auth'
        : 'ember-simple-auth-offline';
      return session.authenticate(
        config[authenticatorConfigName].authenticator,
        login, pass, store);
    }
  }
});