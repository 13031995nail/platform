import $ from 'jquery';
import __ApplicationSerializer 
  from '@timepath/tmobis-core/serializers/application';

export default __ApplicationSerializer.extend({
  /**
  * Field name where object identifier is kept.
  */
  primaryKey: '__PrimaryKey',


  getAttrs: function () {
    let parentAttrs = this._super();
    let attrs = {

    };

    return $.extend(true, {}, parentAttrs, attrs);
  },
  init: function () {
    this.set('attrs', this.getAttrs());
    this._super(...arguments);
  }
});
