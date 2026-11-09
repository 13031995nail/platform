import ActualSerializer from '@timepath/tmobis-core/serializers/actual-entity';

export default ActualSerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      organizationType: { serialize: 'odata-id', deserialize: 'records' },
      hierarchy: { serialize: 'odata-id', deserialize: 'records' },
      town: { serialize: 'odata-id', deserialize: 'records' },
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
