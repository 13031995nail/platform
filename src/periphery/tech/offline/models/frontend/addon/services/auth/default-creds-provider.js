import { inject as service } from '@ember/service';
import DefaultCredsProvider
  from '@timepath/tmobis-core/services/auth/default-creds-provider';

/**
 * @description Сервис автоподставки данных аутентификации
 */
export default DefaultCredsProvider.extend({
  offlineUser: service('offline/offline-user'),

  /**
   * Получить данные аутентификации
   * @returns автоподставленные данные аутентификации
   */
  async initCreds() {
    const offlineUser = this.offlineUser,
          remembered = await offlineUser.initRememberedUser();
    return {
      login: remembered.get('userName'),
      pwd: remembered.get('pwd')
    };
  }
});