import $ from 'jquery';
import { buildValidations } from 'ember-cp-validations';
import DS from 'ember-data';
import { validator } from 'ember-cp-validations';
import { attr } from 'ember-flexberry-data/utils/attributes';
import ActualModel from '@timepath/tmobis-core/models/actual-object';
import { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/actual-entity';

let ValidationRules = {
  dateBegin: {
    validators: [
      validator('ds-error'),
      validator('date'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp.validations.dateBegin.__caption__'
      }),
    ],
  },
  dateEnd: {
    validators: [
      validator('ds-error'),
      validator('date'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp.validations.dateEnd.__caption__'
      }),
    ],
  },
}
const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale'],
});

let Model = ActualModel.extend(Validations, {
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  dateBegin: DS.attr('date'),
  dateEnd: DS.attr('date'),
  name: DS.attr('string'),
  isCurrent: DS.attr('boolean', { defaultValue: false }),

  isCurrentCompute: function () {
    if (!this.get('actuality') && this.get('isCurrent')) {
      this.set('isCurrent', false);
    }
  }.observes('actuality'),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Resources',
  _parentModelName: 'actual-object'
});

Model.defineProjection('AuditView', 'awp', {
  dateBegin: attr('Дата начала', { index: 0 }),
  dateEnd: attr('Дата конца', { index: 1 }),
  isCurrent: attr('Текущий', { index: 2 }),
  actuality: attr('Актуальность', { index: 3, hidden: true }),
});

Model.defineProjection('AwpE', 'awp', {
  dateBegin: attr('Дата начала', { index: 0 }),
  dateEnd: attr('Дата конца', { index: 1 }),
  isCurrent: attr('Текущий', { index: 2 }),
  actuality: attr('Актуальность', { index: 3, hidden: true }),
  name: attr('', { index: 4, hidden: true }),
});

Model.defineProjection('AwpL', 'awp', {
  dateBegin: attr('Дата начала', { index: 0 }),
  dateEnd: attr('Дата конца', { index: 1 }),
  isCurrent: attr('Текущий', { index: 2 }),
  actuality: attr('Актуальность', { index: 3, hidden: true }),
  name: attr('', { index: 4, hidden: true }),
});

export default Model;
