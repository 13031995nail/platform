import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/base-model';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import DS from 'ember-data';

let ValidationRules = {
  awp: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp-preparing-audit-record.validations.awp.__caption__',
      }),
    ],
  },
  workObject: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp-preparing-audit-record.validations.workObject.__caption__',
      }),
    ],
  }
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});

const Model = BaseModel.extend(Validations, {
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),

  evaluationSheetDate: DS.attr('date'),
  hasEvaluationSheetProblem: DS.attr('boolean', { defaultValue: false }),
  evaluationSheetString: DS.attr('string'),
  indexReadiness: DS.attr('decimal'),
  indexPartnerReadiness: DS.attr('decimal'),
  partnerReadiness: DS.attr('partner-readiness-level'),
  passportDate: DS.attr('date'),
  readinessActDate: DS.attr('date'),
  awp: DS.belongsTo('awp', { inverse: null, async: false }),
  awpReadinessPassport: DS.belongsTo('awp-readiness-passport', { inverse: null, async: false }),
  evaluationSheet: DS.belongsTo('evaluation-sheet', { inverse: null, async: false }),
  readinessAct: DS.belongsTo('rediness-act', { inverse: null, async: false }),
  workObject: DS.belongsTo('work-object', { inverse: null, async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'awp-preparing-audit-record', {
  evaluationSheetDate: attr(''),
  indexReadiness: attr(''),
  indexPartnerReadiness: attr(''),
  partnerReadiness: attr(''),
  passportDate: attr(''),
  readinessActDate: attr(''),
  awp: belongsTo('awp', '', {}),
  awpReadinessPassport: belongsTo('awp-readiness-passport', '', {}),
  evaluationSheet: belongsTo('evaluation-sheet', '', {}),
  readinessAct: belongsTo('rediness-act', '', {}),
  workObject: belongsTo('work-object', '', {}),
});
Model.defineProjection('AwpPreparingAuditRecordL', 'awp-preparing-audit-record', {
  workObject: belongsTo('work-object', '', {
    name: attr('Объект', { index: 5 }),
    objectFunction: belongsTo('object-function', '', {
      name: attr('Назначение объекта', { index: 7 })
    }, { index: 6, hidden: true }),
    organization: belongsTo('organization', '', {
      name: attr('Организация ', { index: 4 }),
      hierarchy: belongsTo('organization', '', {
        name: attr('Муниципальное образование', { index: 3 })
      }, { index: 2, hidden: true }),
      organizationType: belongsTo('organization-type', '', {
        name: attr('Тип организации', { index: 3, hidden: true })
      }, { index: 2, hidden: true }),
    }, { index: 1, hidden: true }),
  }, { index: 0, hidden: true }),
  evaluationSheetDate: attr('Оценочный лист', { index: 8, hidden: true }),
  hasEvaluationSheetProblem: attr('', { index: 8, hidden: true }),
  evaluationSheetString: attr('Оценочный лист', { index: 8 }),
  readinessActDate: attr('Акт готовности', { index: 9 }),
  passportDate: attr('Паспорт готовности', { index: 10 }),
  indexReadiness: attr('Индекс готовности объекта', { index: 11 }),
  indexPartnerReadiness: attr('Индекс готовности лица', { index: 12 }),
  partnerReadiness: attr('Уровень готовности лица', { index: 13 }),
  awp: belongsTo('awp', 'ОЗП', {
    dateBegin: attr('', { index: 15, hidden: true }),
    dateEnd: attr('', { index: 16, hidden: true }),
    name: attr('ОЗП', { index: 17, hidden: true })
  }, { index: 14, hidden: true }),
});
Model.defineProjection('AwpPreparingAuditRecordE', 'awp-preparing-audit-record', {
  workObject: belongsTo('work-object', '', {
    name: attr('Объект', { index: 5 }),
    objectFunction: belongsTo('object-function', '', {
      name: attr('Назначение объекта', { index: 7 })
    }, { index: 6, hidden: true }),
    organization: belongsTo('organization', '', {
      name: attr('Организация ', { index: 4 }),
      hierarchy: belongsTo('organization', '', {
        name: attr('Муниципальное образование', { index: 3 })
      }, { index: 2, hidden: true }),
      organizationType: belongsTo('organization-type', '', {
        name: attr('Тип организации', { index: 3, hidden: true }),
        shortName: attr('Тип организации', { index: 3, hidden: true })
      }, { index: 2, hidden: true }),
    }, { index: 1, hidden: true }),
  }, { index: 0, hidden: true }),
  evaluationSheetDate: attr('Оценочный лист', { index: 8 }),
  readinessActDate: attr('Акт готовности', { index: 9 }),
  passportDate: attr('Паспорт готовности', { index: 10 }),
  indexReadiness: attr('Индекс готовности объекта', { index: 11 }),
  indexPartnerReadiness: attr('Индекс готовности лица', { index: 12 }),
  partnerReadiness: attr('Уровень готовности лица', { index: 13 }),
  awp: belongsTo('awp', '', {
    dateBegin: attr('', { index: 0 }),
    dateEnd: attr('', { index: 1 }),
    name: attr('', { index: 4, hidden: true }),
  }, { index: 14, hidden: true }),
  awpReadinessPassport: belongsTo('awp-readiness-passport', '', {
    actNumber: attr('', { index: 0 }),
    createTime: attr('', { index: 1 }),
  }),
  evaluationSheet: belongsTo('evaluation-sheet', '', {
    actNumber: attr('', { index: 0 }),
    createTime: attr('', { index: 1 }),
  }),
  readinessAct: belongsTo('rediness-act', '', {
    actNumber: attr('', { index: 0 }),
    createTime: attr('', { index: 1 }),
  }),
});

export default Model;
