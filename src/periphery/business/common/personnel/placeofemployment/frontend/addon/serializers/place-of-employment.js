import ActualSerializer from '@timepath/tmobis-core/serializers/actual-entity';

export default ActualSerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      organization: { serialize: 'odata-id', deserialize: 'records' },
      position: { serialize: 'odata-id', deserialize: 'records' },
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