import $ from 'jquery';
import BaseSerializer from '@timepath/tmobis-core/serializers/application';

export default BaseSerializer.extend({
  getAttrs() {
    let parentAttrs = this._super();
    let attrs = {
      awp: { serialize: 'odata-id', deserialize: 'records' },
      awpReadinessPassport: { serialize: 'odata-id', deserialize: 'records' },
      evaluationSheet: { serialize: 'odata-id', deserialize: 'records' },
      readinessAct: { serialize: 'odata-id', deserialize: 'records' },
      workObject: { serialize: 'odata-id', deserialize: 'records' },  
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },
  init: function () {
    this.set('attrs', this.getAttrs());
    this._super(...arguments);
  }
});
