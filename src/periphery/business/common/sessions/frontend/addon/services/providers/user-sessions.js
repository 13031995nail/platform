import UserSessionSetDeviceData from '../../messages/user-session-set-device-data';
import UserSessionStopSession from '../../messages/user-session-close-session';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate, DatePredicate }
  from 'ember-flexberry-data/query/predicate';
import Condition from 'ember-flexberry-data/query/condition';
import QueryBuilder from 'ember-flexberry-data/query/builder';
import Service, { inject as service } from '@ember/service';
import { isEmpty } from '@ember/utils';

export default Service.extend({
  i18n: service(),
  modes: service(),
  store: service(),
  modals: service(),
  session: service(),
  messageDispatcher: service('message-dispatcher'),

  async onAuthSuccessful() {
    await this.setDeviceData();
  },

  /**
   * Сохранить данные об устройстве текущего
   * авторизованного пользователя в активную сессию.
   * @returns {Promise}
   */
  setDeviceData() {
    const deviceInfo = this.getDeviceInfo();
    return this.setDeviceDataInternal(deviceInfo);
  },

  getDeviceInfo() {
    const runningOnDeviceApp = this.get('modes.runningOnDeviceApp');
    let deviceInfo = undefined;
    if (runningOnDeviceApp && window.deviceInfo) {
      deviceInfo = `${window.deviceInfo.model} 
        ${window.deviceInfo.uuid} 
        ${window.deviceInfo.version} 
        ${window.deviceInfo.serial} 
        ${window.deviceInfo.manufacturer}`;
    } else {
      deviceInfo = window.navigator.userAgent;
    }

    return deviceInfo;
  },

  /** 
   * @protected
   * @returns {Promise}
   */
  setDeviceDataInternal(deviceInfo) {
    const message = new UserSessionSetDeviceData(deviceInfo);
    return this.messageDispatcher.sendMessage(message);
  },

  /**
   * Получить функцию ограничения на все сессии текущего пользователя.
   * @returns {Promise}
   */
  getMySessionsLimit() {
    const userId = this.get('session.data.authenticated.user'),
          limit = new ComplexPredicate(Condition.And, 
            new SimplePredicate('user.id', FilterOperator.Eq, userId),
            new SimplePredicate('actuality', FilterOperator.Eq, true));

    return limit;
  },

  /**
   * Получить функцию ограничения на все сессии текущего пользователя.
   * @returns {Promise}
   */
  getAllSessionsLimit(
    base = undefined,
    dateFrom = undefined,
    dateTo = undefined,
    useArchive = false) {
    const conditions = [];
    
    if (base) {
      conditions.push(base);
    }

    if (!isEmpty(dateFrom)) {
      conditions.push(new DatePredicate(
        'startedAt', FilterOperator.Geq, dateFrom, true));
    }

    if (!isEmpty(dateTo)) {
      conditions.push(new DatePredicate(
        'startedAt', FilterOperator.Leq, dateTo, true));
    }
    
    if (!useArchive) {
      conditions.push(new SimplePredicate(
        'actuality', FilterOperator.Eq, true));
    }

    return conditions.length === 0
      ? base
      : conditions.length === 1
        ? conditions[0]
        : new ComplexPredicate(Condition.And, ...conditions);
  },

  /**
   * Получить все сессии текущего пользователя.
   * @returns {Promise}
   */
  getMySessions() {
    const store = this.store,
          limit = this.getMySessionsLimit(),
          builder = new QueryBuilder(store)
            .from('user-session')
            .where(limit)
            .selectByProjection('SessionD');

    return store.query('user-session', builder.build());
  },

  /**
   * Удалить текущую сессию
   * @returns {Promise}
   */
  stopCurrentSession() {
    const sessionId = this.get('session.data.authenticated.sessionId');
    return this.stopSession(sessionId);
  },

  /**
   * Удалить сессию с заданным ИД.
   * Только того же пользователя, либо администратором.
   * Текущую сессию нельзя завершить этим методом.
   * @param {String} sessionId идентификатор сессии
   * @returns {Promise}
   */
  async stopOtherSession(sessionId) {
    const i18n = this.i18n,
          modals = this.modals,
          currentSessionId = this.get('session.data.authenticated.sessionId');

    if (sessionId === currentSessionId) {
      modals.showAlert({
        message: i18n.t('features.userSessions.closeSessionFailSelf'),
        title: i18n.t('features.userSessions.closeSessionFailTitle')
      });
      return;
    }

    await this.stopSession(sessionId);
  },

  /**
   * Удалить сессию с заданным ИД.
   * Только того же пользователя.
   * @param {String} sessionId идентификатор сессии
   * @returns {Promise}
   * @protected
   */
  async stopSession(sessionId) {
    const message = new UserSessionStopSession(sessionId);
    try {
      await this.messageDispatcher.sendMessage(message);
    } catch (ex) {
      console.log(ex);
    }
  },
});
