import Component from '@ember/component';
import { inject as service } from '@ember/service';
import { computed } from '@ember/object';
import { isEmpty } from '@ember/utils';

export default Component.extend({
  offlineBackupLoader: service('offline/offline-backup-loader'),
  currentPath: undefined,

  loadHasErrors: computed('offlineBackupLoader.loadErrors', 'currentPath', function () {
    let errors = this.get('offlineBackupLoader.loadErrors');
    return !isEmpty(errors) && this.currentPath === 'desktop';
  }),

  actions: {
    showFullLoadErrors() {
      let loadErrors = this.get('offlineBackupLoader.loadErrors'),
        offlineError = this.get('offlineError');

      offlineError.offlineMessage(null, 'Не удалось загрузить бэкапы справочников:', loadErrors);
    }
  }
});