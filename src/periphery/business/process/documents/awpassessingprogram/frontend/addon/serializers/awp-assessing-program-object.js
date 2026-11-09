import $ from 'jquery';
import BaseSerializer
  from '@timepath/tmobis-core/serializers/application';

export default BaseSerializer.extend({
  getAttrs() {
    let parentAttrs = this._super();
    let attrs = {
      program: { serialize: 'odata-id', deserialize: 'records' },
      workObject: { serialize: 'odata-id', deserialize: 'records' },
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