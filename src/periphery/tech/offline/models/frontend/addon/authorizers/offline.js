import { isEmpty } from '@ember/utils';
import Base from 'simple-auth/authorizers/base';
export default Base.extend({
  authorize: function(jqXHR, requestOptions) {
    let accessToken = this.get('session.content.secure.token');
    if (this.get('session.isAuthenticated') && !isEmpty(accessToken)) {
      jqXHR.setRequestHeader('Authorization', 'Bearer ' + accessToken);
    }
  }
});