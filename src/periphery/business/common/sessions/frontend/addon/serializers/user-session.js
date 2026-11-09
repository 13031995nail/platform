import ApplicationSerializer from '@timepath/tmobis-core/serializers/application';

export default ApplicationSerializer.extend({
  primaryKey: '__PrimaryKey',

  getAttrs() {
    const parentAttrs = this._super();
    const attrs = {
      user: { serialize: 'odata-id', deserialize: 'records' }
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },

  init: function () {
    this.set('attrs', this.getAttrs());
    this._super(...arguments);
  }
});
