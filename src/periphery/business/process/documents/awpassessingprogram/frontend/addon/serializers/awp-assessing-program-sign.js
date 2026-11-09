import $ from 'jquery';
import ActSign
  from '@timepath/tmobis-business-process-acts-actsign/serializers/act-sign';

export default ActSign.extend({
  getAttrs() {
    let parentAttrs = this._super();
    let attrs = {
      act: { serialize: 'odata-id', deserialize: 'records' }
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
