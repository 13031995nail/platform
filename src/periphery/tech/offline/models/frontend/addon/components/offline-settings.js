import Component from '@ember/component';
import { inject as service } from '@ember/service';

export default Component.extend({
  offlineBackupManager: service('offline/offline-backup-manager'),

  actions: {
    async exportLocaleDB() {
      await this.offlineBackupManager.exportLocaleDB();
    },

    async importLocaleDB() {
      await this.offlineBackupManager.importLocaleDB();
    },

    reloadDictionaries() {
      this.offlineBackupManager.reloadDictionaries();
    },

    syncDictionaries() {
      this.offlineBackupManager.syncDictionaries();
    },

    syncPermissions() {
      this.offlineBackupManager.syncPermissions();
    }
  }
});