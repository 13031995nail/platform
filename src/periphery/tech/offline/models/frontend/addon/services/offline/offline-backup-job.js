import config from 'ember-get-config';
import { isEmpty } from '@ember/utils';
import Service, { inject as service } from '@ember/service';
import { cancel, later } from '@ember/runloop';

export default Service.extend({
  checkUpdatesInterval: undefined,
  checkUpdatesTimer: undefined,
  
  session: service(),
  offlineBackupManager: service('offline/offline-backup-manager'),

  init() {
    this._super(...arguments);
    if (isEmpty(config.APP.checkUpdatesInterval)) {
      throw new Error("checkUpdatesInterval can not be null, check your environment");
    }
    else {
      this.set('checkUpdatesInterval', config.APP.checkUpdatesInterval);
    }
  },

  initializeTimer() {
    let checkUpdatesTimer = this.checkUpdatesTimer;
    cancel(checkUpdatesTimer);

    checkUpdatesTimer = later(this, this.initializeTimer, this.checkUpdatesInterval);
    this.set('checkUpdatesTimer', checkUpdatesTimer);

    this.checkUpdates();
  },

  checkUpdates() {
    if (this.session.isAuthenticated) {
      this.offlineBackupManager.syncDictionaries();
    }
  },
})