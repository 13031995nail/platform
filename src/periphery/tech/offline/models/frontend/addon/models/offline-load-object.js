import DS from 'ember-data';
import BaseModel from 'ember-flexberry-data/models/model';
import { attr } from 'ember-flexberry-data/utils/attributes';

let Model = BaseModel.extend({
  objectsModelName: DS.attr('string'),
  objectsLoadDate: DS.attr('date'),
  objectsLoadByEditDate: DS.attr('date'),
  objectsLastSuccessLoadDate: DS.attr('date'),
  objectsIsLoaded: DS.attr('boolean')
});

Model.defineProjection('OfflineLoadObjectL', 'offline-load-object', {
  objectsModelName: attr(),
  objectsLoadDate: attr(),
  objectsLoadByEditDate: attr(),
  objectsLastSuccessLoadDate: attr(),
  objectsIsLoaded: attr()
});

export default Model;
