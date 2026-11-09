import DS from 'ember-data';
import BaseModel from 'ember-flexberry-data/models/model';
import { attr } from 'ember-flexberry-data/utils/attributes';

let Model = BaseModel.extend({
  login: DS.attr('string'),
  object: DS.attr('string'),
  actions: DS.attr('string')
});

Model.defineProjection('OfflinePermissionL', 'offline-permission', {
    login: attr(),
    object: attr(),
    actions: attr()
});

export default Model;
