import BaseSerializer from '@timepath/tmobis-core/serializers/employee';


export default BaseSerializer.extend({
  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {
      placeOfEmployment: { serialize: false, deserialize: false }
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },
});