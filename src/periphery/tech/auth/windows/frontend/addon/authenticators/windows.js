import Base from 'ember-simple-auth/authenticators/base';
import config from 'ember-get-config';
import { isEmpty } from '@ember/utils'; 
import { resolve } from 'rsvp';

export default Base.extend({
  async restore(data) {
    if (!isEmpty(data.token)) {
      return data;
    } else {
      throw new Error("Токен не обнаружен");
    }
  },

  async authenticate() {
    const url = `${config.APP.backendUrls.root}/api/auth/validate`;

    try {
      let response = await fetch(url, { credentials: 'include' });

      if (!response.ok) {
        throw new Error('Not authorized via Windows');
      }

      let data = await response.json();

      if (data && data.access_token) {
        return data;
      } else {
        throw new Error('No access token returned');
      }
    } catch (e) {
      throw {
        error: 'Windows authentication error',
        error_description: e.message || e
      };
    }
  },

  invalidate() {
    return resolve();
  }
});