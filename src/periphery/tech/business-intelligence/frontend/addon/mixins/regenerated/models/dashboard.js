import Mixin from '@ember/object/mixin';
import DS from 'ember-data';
import { attr } from 'ember-flexberry-data/utils/attributes';

export let Model = Mixin.create({
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  note: DS.attr('string'),
  tmobisDashboardId: DS.attr('string'),
  metabaseDashboardId: DS.attr('string'),
  actuality: DS.attr('boolean'),
  name: DS.attr('string'),
});

export let defineBaseModel = function (modelClass) {
  modelClass.reopenClass({
    _parentModelName: 'tmobis-core-objects-dictionary'
  });
};

export let defineProjections = function (modelClass) {
  modelClass.defineProjection('DashboardE', 'timepath-business-intelligence-dashboard', {
    tmobisDashboardId: attr('ID дашборда в tmobis', { index: 0 }),
    metabaseDashboardId: attr('ID дашборда в metabase', { index: 1 }),
    note: attr('Примечание', { index: 2 }),
    name: attr('Наименование дашборда', { index: 3 }),
    actuality: attr('Актуальность', { index: 4 }),
  });
};