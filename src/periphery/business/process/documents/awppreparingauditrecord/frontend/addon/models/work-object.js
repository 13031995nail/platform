import WorkObjectModel from '@timepath/tmobis-business-common-address-workobject/models/work-object';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';

let Model = WorkObjectModel.extend({
});

Model.defineProjection('WorkObjectLookUp', 'work-object', {
  name: attr('Объект', { index: 0 }),
  organization: belongsTo('organization', '', {
    name: attr('Организация', { index: 1 }),
    hierarchy: belongsTo('organization', '', {
      name: attr('МО', { index: 3 })
    }, { hidden: true }),
  }, { hidden: true }),
  objectFunction: belongsTo('object-function', '', {
    name: attr('Назначение объекта', { index: 2 })
  }, { hidden: true }),
});

export default Model;
