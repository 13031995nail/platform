import $ from 'jquery';
import BaseSerializer from '@timepath/tmobis-core/serializers/application';

export default BaseSerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      signType: { serialize: 'odata-id', deserialize: 'records' },
      signRole: { serialize: 'odata-id', deserialize: 'records' },
      employee: { serialize: 'odata-id', deserialize: 'records' },
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },
  init: function () {
    this.set('attrs', this.getAttrs());
    this._super(...arguments);
  },
  /**
  * Field name where object identifier is kept.
  */
  primaryKey: '__PrimaryKey'
});
