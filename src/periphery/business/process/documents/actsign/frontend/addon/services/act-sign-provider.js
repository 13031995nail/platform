import { inject as service, default as Service } from '@ember/service';
import config from 'ember-get-config';

export default Service.extend({
  entityLoader: service('entity-loader'),
  store: service(),

  cancelSigning(signModel, documentId) {
    const root = config.APP.backendUrls.root;
    const url = `${root}/api/ActSigning/${encodeURIComponent(signModel)}/${encodeURIComponent(documentId)}/cancel`;
    return this.get('store').adapterFor('application').ajax(url, 'POST', { data: {} });
  },

  getCellComponent: function (attr, bindingPath, model, cellComponent) {
    if (bindingPath === 'signature') {
      cellComponent.componentName = 'olv-file-preview';
      cellComponent.componentProperties = {
        enablePreview: true
      };
      return cellComponent;
    }
    else if (bindingPath === 'signDateString' || bindingPath === 'signRole.name') {
      return {
        componentName: 'object-list-view-cell',
      };
    }
    cellComponent.componentName = 'flexberry-textbox';
    return cellComponent;
  },
});
