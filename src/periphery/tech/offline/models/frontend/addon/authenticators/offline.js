import { isEmpty } from '@ember/utils';
import { resolve } from 'rsvp';
import { isNone } from '@ember/utils';
import { inject as service } from '@ember/service';
import OAuth2PasswordGrant from 'ember-simple-auth/authenticators/oauth2-password-grant';

export default OAuth2PasswordGrant.extend({
  i18n: service(),
  offlineUserService: service('offline/offline-user'),

  async restore(data) {
    if (!isEmpty(data.token)) {
      return data;
    } else {
      throw "Токен не обнаружен";
    }
  },

  async authenticate(login, password) {
    const offlineUserService = this.offlineUserService;
    const offlineUser = await offlineUserService.getOfflineUser({ userName: login, pwd: password });
    
    const errorMessage = this.get('i18n').t('forms.login.on-auth-failed.message');
    
    if (isNone(offlineUser)) {
      throw {
        error: 'Authentication error',
        error_description: errorMessage
      };
    }

    const offlineData = offlineUser.get('userData');
    if (offlineData['expires_in'] <= new Date().getTime()) {
      throw {
        error: 'Authentication error',
        error_description: errorMessage
      };
    }

    return offlineData;
  },

  invalidate() {
    console.log('invalidate...');
    return resolve();
  }
});
