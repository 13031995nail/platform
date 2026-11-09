import ApplicationSerializer
  from '@timepath/tmobis-core/serializers/application-offline';

export default ApplicationSerializer.extend({
  getAttrs() {
    const parentAttrs = this._super();
    const attrs = {
      user: { serialize: 'id', deserialize: 'records' }
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },

  init: function () {
    this.set('attrs', this.getAttrs());
    this._super(...arguments);
  }
});
