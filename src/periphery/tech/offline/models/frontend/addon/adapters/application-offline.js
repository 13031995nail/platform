import config from 'ember-get-config';
import FilesUpdatingAdapterMixin
  from '@timepath/tmobis-core/mixins/files-updating-adapter';
import OfflineAdapter from 'ember-flexberry-data/adapters/offline';

export default OfflineAdapter.extend(FilesUpdatingAdapterMixin, {
  dbName: config.APP.offline.dbName,

  /**
   * Переопределение метода из миксина files-updating-adapter.
   */
  async getDeleteFilePromise(id) {
    const fileService = this.file;
    try {
      await fileService.deleteFileFromDevice(id);
      fileService.forget(id);
    } catch (ex) {
      console.error(ex);
    }
  },

  /**
   * Переопределение метода из миксина files-updating-adapter.
   */
  async getEnsureFilePromise(id) {
    const fileService = this.file;
    try {
      await fileService.ensureFileIsOnDevice(id);
    } catch (ex) {
      console.error(ex);
    }
  },
});
