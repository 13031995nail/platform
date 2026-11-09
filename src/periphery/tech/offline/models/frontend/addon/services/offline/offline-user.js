import Service, { inject as service } from '@ember/service';
import { getOwner } from '@ember/application';
import { merge } from '@ember/polyfills';

/**
 * @description сервис работы с офлайн пользователем
 */
export default Service.extend({
  store: service(),
  session: service(),
  dexie: service(),
  localStore: undefined,

  init() {
    this._super(...arguments);
    this.set('localStore', getOwner(this).lookup('store:local'));
  },

  async onAuthSuccessful(options) {
    await this.updateOfflineUser(options);
  },

  async onSessionInvalidated() {
    this.clearUsers;
  },

  /** 
   * @protected
   * @description создать или обновить офлайн пользователя после аутентификации
   */
  async updateOfflineUser(options) {
    // Запишем текущего пользователя и его сессию в store,
    // используя при этом оффлайн-хранилище.
    const pwd = options.pwd,
          rememberMe = options.rememberMe,
          onlineModeOn = ('onlineModeOn' in options)
            ? options.onlineModeOn
            : true,
          store = this.store,
          sessionInfo = this.get('session.data.authenticated'),
          allOfflineUsers = await store.query(
            'offline-user', { useOnlineStore: false }),
          offlineUsers = allOfflineUsers.filterBy(
            'userName', sessionInfo.userName);

    if (offlineUsers.get('length') === 0) {
      await this.createOfflineUser(sessionInfo, pwd, rememberMe, onlineModeOn);
      if (rememberMe) {
        allOfflineUsers.setEach('rememberMe', false);
        return allOfflineUsers.save();
      }
    } else {
      // Если в офлайн хранилище уже есть такой пользователь,
      // то обновим его данные, которые могли измениться
      const offlineUser = offlineUsers.get('firstObject');
      offlineUser.set('userName', sessionInfo.userName);
      offlineUser.set('offlineModeAvailable', !onlineModeOn);
      offlineUser.set('pwd', pwd);
      offlineUser.set('serializedData', JSON.stringify(sessionInfo));
      if (rememberMe) {
        allOfflineUsers.setEach('rememberMe', false);
        offlineUser.set('rememberMe', true);
        allOfflineUsers.save();
      } else {
        offlineUser.set('rememberMe', false);
        offlineUser.save();
      }
    }
  },
  
  /** 
   * @protected
   * @description создать офлайн пользователя с указанными данными
   */
  createOfflineUser(sessionInfo, passwd, rememberMe, onlineModeOn) {
    const store = this.store,
          newOfflineUser = store.createRecord('offline-user', {
            userName: sessionInfo.userName,
            pwd: passwd,
            rememberMe: rememberMe || false,
            isOnline: true,
            offlineModeAvailable: !onlineModeOn,
            serializedData: JSON.stringify(sessionInfo)
          }, false);
    return newOfflineUser.save();
  },

  /**
   * @public
   * @description получить офлайн пользователя из ЛБД по логину
   * @param {Object} query запрос с полями для поиска
   * @returns офлайн пользователь
   */
  async getOfflineUser(query = {}) {
    const store = this.store;
    query = merge(query, { useOnlineStore: false });
    const users = await store.query('offline-user', query);
    return users.get('firstObject');
  },

  /**
   * @public
   * @description найти и вернуть одного запомненного пользователя,
   * если есть другие - снять с них флаги запоминания
   * @returns 
   */
  async initRememberedUser() {
    const store = this.store,
          offlineUsers = await store.query('offline-user', {
            remembered: true,
            useOnlineStore: false
          }),
          length = offlineUsers.get('length'),
          offlineUser = offlineUsers.get('firstObject');

    if (length > 1) {
      offlineUsers.removeObject(offlineUser);
      offlineUsers.setEach('remembered', false);
      await offlineUsers.save();
    }

    return offlineUser;
  },

  /**
   * @public
   * @description Установить состояние онлайн/оффлайн для текущего пользователя.
   * @param {boolean} isOnline true, если пользователь онлайн; false, если оффлайн.
   * @returns
   */
  async setUserOnlineState(isOnline) {
    let offlineUser = await this.getOfflineUser();
    if (offlineUser) {
      offlineUser.set('isOnline', isOnline);
      await offlineUser.save();
    }
  },

  /**
   * @public
   * @description Найти и вернуть состояние (онлайн/оффлайн) текущего пользователя.
   * @returns
   */
  async getUserOnlineState() {
    let offlineUser = await this.getOfflineUser();
    return offlineUser ? offlineUser.get('isOnline') : true;
  },

  /** 
   * @protected
   * @description очистить офлайн пользователей в ЛБД
   */
  clearUsers() {
    const localStore = this.localStore,
          localAdapterForUsers = localStore.adapterFor('offline-user');
    localAdapterForUsers.clear('offline-user');
  }
});