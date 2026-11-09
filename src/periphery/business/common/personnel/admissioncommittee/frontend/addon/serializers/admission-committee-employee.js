import DictionarySerializer from '@timepath/tmobis-core/serializers/actual-entity';

export default DictionarySerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      employee: { serialize: 'odata-id', deserialize: 'records' },
      admissionCommittee: { serialize: 'odata-id', deserialize: 'records' },
      placeOfEmployment: { serialize: false, deserialize: false }
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
