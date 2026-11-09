import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-core/models/base-model';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import DS from 'ember-data';
import { buildValidations, validator } from 'ember-cp-validations';
import { computed } from '@ember/object';

let ValidationRules = {
  program: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp-assessing-program-evaluation-schedule.validations.program.__caption__',
      }),
    ],
  },
  objectFunction: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp-assessing-program-evaluation-schedule.validations.objectFunction.__caption__',
      }),
    ],
  },
  finishDate: {
    validators: [
    validator('date', {
      after: computed('model.startDate', function () {
        return this.get('model.startDate');
      }),
      message(type) {
        if (type === 'after') {
          return 'Дата окончания должна быть не раньше даты начала';
        }
      }
    })
  ]}
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  startDate: DS.attr('date'),
  finishDate: DS.attr('date'),
  objectsCount: DS.attr('number'),
  objectFunction: DS.belongsTo('object-function', { inverse: null, async: false }),
  program: DS.belongsTo('awp-assessing-program', { inverse: 'schedules', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'awp-assessing-program-evaluation-schedule', {
  startDate: attr('', { index: 0 }),
  finishDate: attr('', { index: 2 }),
  objectFunction: belongsTo('object-function', '', {}, { index: 9 }),
  program: belongsTo('awp-assessing-program', '', {}, { index: 9 }),
});
Model.defineProjection('EvaluationScheduleD', 'awp-assessing-program-evaluation-schedule', {
  startDate: attr('', { index: 0 }),
  finishDate: attr('', { index: 2 }),
  objectFunction: belongsTo('object-function', '', {
    name: attr('Назначение объектов', { index: 2 }),
  }, { index: 9, hidden: true }),
  program: belongsTo('awp-assessing-program', '', {}, { index: 9 }),
});

export default Model;