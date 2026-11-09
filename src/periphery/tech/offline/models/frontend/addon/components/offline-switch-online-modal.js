import Component from '@ember/component';
import { inject as service } from '@ember/service';

export default Component.extend({
  dexie: service(),
  switchOnline: service('offline/switch-online'),

  init() {
    this._super(...arguments);
  }
});
