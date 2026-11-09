import BaseSecurity from '@timepath/tmobis-core/services/security';

export default BaseSecurity.extend({
  loadPermissionToCache(modelName) {
    const self = this,
          cache = self.get('modelPermissions');

    return self.getOfflinePermissions().then((records) => {
      if (records.content.length > 0) {
        // Если запись о полномочиях найдена в оффлайн-хранилище, используем ее.
        cache[modelName] = records.objectAt(0).get('actions').split(',');
        return Promise.resolve(cache[modelName]);
      } else {
        return self.getOnlinePermissions(modelName);
      }
    });
  },

  getOfflinePermissions(modelName) {
  const self = this,
      store = self.get('store'),
      session = self.get('session');

    return store.query('offline-permission', {
      login: session.get('data.authenticated.userName'),
      object: modelName,
      useOnlineStore: false
    });
  }
});