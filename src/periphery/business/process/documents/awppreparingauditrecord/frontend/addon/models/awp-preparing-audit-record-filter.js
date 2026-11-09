import DS from 'ember-data';
import EmberFlexberryDataModel from 'ember-flexberry-data/models/model';
import { A } from '@ember/array';
import { computed } from '@ember/object';

export default EmberFlexberryDataModel.extend({
  awp: DS.belongsTo('awp', { inverse: null, async: false }),
  organization: DS.belongsTo('organization', { inverse: null, async: false }),
  objectFunction: DS.belongsTo('object-function', { inverse: null, async: false }),
  organizationType: DS.belongsTo('organization-type', { inverse: null, async: false }),
  workObjectToCreateRecord: DS.belongsTo('work-object', { inverse: null, async: false }),
  // Список допустимых объектов (строковые GUID'ы).
  availableObjectIds: computed(function () { return A(); }),
});
