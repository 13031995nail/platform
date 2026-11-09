import DS from 'ember-data';
import ActualEntity from '@timepath/tmobis-core/models/actual-entity';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';

let Model = ActualEntity.extend({
  login: DS.attr('string'),
  startedAt: DS.attr('date'),
  closedAt: DS.attr('date'),
  deviceInfo: DS.attr('string'),
  ip: DS.attr('string'),
  user: DS.belongsTo('user', { inverse: null, async: false })
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Sessions.Flexberry.Models'
});

Model.defineProjection('AuditView', 'user-session', {
  login: attr('', { index: 0 }),
  startedAt: attr('', { index: 1 }),
  actuality: attr('', { index: 1 }),
  deviceInfo: attr('', { index: 2 }),
  user: belongsTo('user', '', {
  }, { hidden: true }),
});

Model.defineProjection('SessionD', 'user-session', {
  login: attr('', { index: 0, hidden: true }),
  startedAt: attr('Дата начала', { index: 1 }),
  deviceInfo: attr('Устройство', { index: 2 }),
  ip: attr('IP', { index: 3 }),
  actuality: attr('', { hidden: true }),
});

Model.defineProjection('SessionL', 'user-session', {
  login: attr('Пользователь', { index: 0 }),
  startedAt: attr('Дата начала', { index: 1 }),
  deviceInfo: attr('Устройство', { index: 2 }),
  ip: attr('IP', { index: 3 }),
  actuality: attr('Активна', { index: 4 }),
  closedAt: attr('Дата закрытия', { index: 5 }),
});

export default Model;
