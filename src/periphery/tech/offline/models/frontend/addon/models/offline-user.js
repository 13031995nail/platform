import DS from 'ember-data';
import BaseModel from '@timepath/tmobis-core/models/base-model';
import { attr } from 'ember-flexberry-data/utils/attributes';
import { computed } from '@ember/object';

let Model = BaseModel.extend({
  userName: DS.attr('string'),
  pwd: DS.attr('string'),
  rememberMe: DS.attr('boolean'),
  isOnline: DS.attr('boolean'),
  offlineModeAvailable: DS.attr('boolean'),
  serializedData: DS.attr('string'),
  userData: computed('serializedData', function () {
    return JSON.parse(this.get('serializedData'));
  }).readOnly(),
});

Model.defineProjection('OfflineUserL', 'offline-user', {
  userName: attr(),
  pwd: attr(),
  rememberMe: attr(),
  isOnline: attr(),
  offlineModeAvailable: attr(),
  serializedData: attr()
});

export default Model;
