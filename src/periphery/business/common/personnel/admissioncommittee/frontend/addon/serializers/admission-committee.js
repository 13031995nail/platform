import DictionarySerializer from '@timepath/tmobis-core/serializers/actual-entity';

export default DictionarySerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      leader: { serialize: 'odata-id', deserialize: 'records' },
      deputyLeader: { serialize: 'odata-id', deserialize: 'records' },
      awp: { serialize: 'odata-id', deserialize: 'records' },
      organization: { serialize: 'odata-id', deserialize: 'records' },
      admissionCommitteeEmployee: { serialize: false, deserialize: 'records' }
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
