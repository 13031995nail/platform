import Component from '@ember/component';
import { inject as service } from '@ember/service';

export default Component.extend({
  offlineBackupManager: service('offline/offline-backup-manager'),

  init() {
    this._super(...arguments);
  }
});
