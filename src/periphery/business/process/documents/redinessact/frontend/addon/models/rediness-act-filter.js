import DS from 'ember-data';
import EmberFlexberryDataModel from 'ember-flexberry-data/models/model';

export default EmberFlexberryDataModel.extend({
  awp: DS.belongsTo('awp', { inverse: null, async: false })
});
