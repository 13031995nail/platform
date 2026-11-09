import { inject as service } from '@ember/service';
import OnlineStore from 'ember-flexberry-data/stores/online-store';

export default OnlineStore.extend({
  syncer: service(),
  offlineState: service('offline-globals'),

  query(modelName, query) {
    let self = this,
      projectionName = !query ? null : query.projectionName;
   return this._super(...arguments).then(function (result) {
     // Если объект загружается по E-проекции, обновляем его в оффлайн-хранилище.
     if (self.get('offlineState.synchronousSyncDownEnabled')) {
       return self.get('syncer').syncDown(result, false, projectionName, { unloadSyncedRecords: false }).then(function () {
         return result;
       });
     }

     return result;
   });
  },
});