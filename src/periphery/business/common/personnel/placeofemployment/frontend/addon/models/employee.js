import BaseModel from '@timepath/tmobis-core/models/employee';
import DS from 'ember-data';

let Model = BaseModel.extend({
  placeOfEmployment: DS.hasMany('place-of-employment', { inverse: null, async: false })
});

export default Model;