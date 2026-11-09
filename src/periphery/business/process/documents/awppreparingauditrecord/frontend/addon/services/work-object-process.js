import Service, { inject as service } from '@ember/service';
import config from 'ember-get-config';

export default Service.extend({
  store: service(),

  request(objectId, method, suffix, data) {
    const root = config.APP.backendUrls.root;
    const url = `${root}/api/WorkObjectProcess/${encodeURIComponent(objectId)}${suffix}`;
    return this.get('store').adapterFor('application').ajax(url, method, { data });
  },

  load(objectId, awpId) {
    return this.request(objectId, 'GET', '', awpId ? { awpId } : {});
  },

  review(objectId, awpId, operation) {
    const query = `?operation=${encodeURIComponent(operation)}` +
      (awpId ? `&awpId=${encodeURIComponent(awpId)}` : '');
    return this.request(objectId, 'POST', `/review${query}`, {});
  },

  remarks(workspace, awpId) {
    return this.request('remarks', 'GET', '', awpId ? { workspace, awpId } : { workspace });
  }
});
