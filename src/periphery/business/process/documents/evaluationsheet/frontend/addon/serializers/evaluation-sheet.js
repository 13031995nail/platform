import $ from 'jquery';
import BaseActSerializer
  from '@timepath/tmobis-business-process-acts-base-act/serializers/base-act';

export default BaseActSerializer.extend({
  getAttrs() {
    let parentAttrs = this._super();
    let attrs = {
      actSigns: { serialize: false, deserialize: 'records' },
      requirements: { serialize: false, deserialize: 'records' },
      workObject: { serialize: 'odata-id', deserialize: 'records' },
      awp: { serialize: 'odata-id', deserialize: 'records' }
    };

    return $.extend(true, {}, parentAttrs, attrs);
  }
});
