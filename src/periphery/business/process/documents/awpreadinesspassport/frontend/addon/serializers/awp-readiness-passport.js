import $ from 'jquery';
import BaseAct
  from '@timepath/tmobis-business-process-acts-base-act/serializers/base-act';

export default BaseAct.extend({
  getAttrs() {
    let parentAttrs = this._super();
    let attrs = {
      awp: { serialize: 'odata-id', deserialize: 'records' },
      readinessAct: { serialize: 'odata-id', deserialize: 'records' },
      actSigns: { serialize: false, deserialize: 'records' },
      objects: { serialize: false, deserialize: 'records' },
    };

    return $.extend(true, {}, parentAttrs, attrs);
  },
});
