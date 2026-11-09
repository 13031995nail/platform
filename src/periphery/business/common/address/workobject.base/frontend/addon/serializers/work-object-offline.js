import __ApplicationSerializer 
  from '@timepath/tmobis-core/serializers/application-offline';

export default __ApplicationSerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      hierarchy: { serialize: 'id', deserialize: 'records' }
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },
  init: function () {
    this.set('attrs', this.getAttrs());
    this._super(...arguments);
  }
});
