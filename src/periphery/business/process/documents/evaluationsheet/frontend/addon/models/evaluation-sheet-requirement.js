import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-core/models/base-model';
import DS from 'ember-data';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';

let ValidationRules = {
  evaluationSheet: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.evaluation-sheet-requirement.validations.evaluationSheet.__caption__'
      })
    ]
  },
  verificationRequirement: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.evaluation-sheet-requirement.validations.verificationRequirement.__caption__'
      })
    ]
  }
  ,
  shortReqName: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.evaluation-sheet-requirement.validations.shortReqName.__caption__'
      })
    ]
  }
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});

const Model = BaseModel.extend(Validations, {
  filesPropertyNames: ['supportingDocFile'],

  reqOrder: DS.attr('number'),
  sortOrder: DS.attr('number'),
  reqNumber: DS.attr('string'),
  shortReqName: DS.attr('string'),
  indicatorName: DS.attr('string'),
  weight: DS.attr('decimal'),
  supportingDocName: DS.attr('string'),
  supportingDocFile: DS.attr('string'),
  calculatedReq: DS.attr('decimal', { defaultValue: 0 }),
  hasEnteredValue: DS.attr('boolean', { defaultValue: false }),
  isCalculated: DS.attr('boolean', { defaultValue: false }),
  isCompletedReq: DS.attr('boolean'),
  problemComment: DS.attr('string'),
  deadline: DS.attr('date'),
  isOptional: DS.attr('boolean', { defaultValue: false }),
  isVerified: DS.attr('boolean'),
  evaluationSheet: DS.belongsTo('evaluation-sheet', { inverse: 'requirements', async: false }),
  verificationRequirement: DS.belongsTo('verification-requierement', { inverse: null, async: false })
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents'
});

Model.defineProjection('EvaluationSheetRequirementD', 'evaluation-sheet-requirement', {
  reqOrder: attr('Порядок проверки', { index: 0 }),
  reqNumber: attr('Номер требования', { index: 1 }),
  shortReqName: attr('Краткое название', { index: 2 }),
  indicatorName: attr('Название показателя', { index: 3 }),
  weight: attr('Вес', { index: 4 }),
  verificationRequirement: belongsTo('verification-requierement', '', {
    name: attr('Требование', { index: 5 })
  }, { index: 5 }),
  supportingDocName: attr('Документ', { index: 6 }),
  supportingDocFile: attr('Приложение', { index: 7 }),
  calculatedReq: attr('Расчёт', { index: 7 }),
  hasEnteredValue: attr('Значение введено', { hidden: true }),
  isCalculated: attr('Рассчитано', { index: 9 }),
  isCompletedReq: attr('Выполнено', { index: 10 }),
  problemComment: attr('Комментарий', { index: 11 }),
  deadline: attr('Срок', { index: 12 }),
  sortOrder: attr('SortOrder', { index: 13, hidden: true }),
  isOptional: attr('Наличие приложения', { index: 14, hidden: true }),
  isVerified: attr('Проверено', { index: 15 }),
});

export default Model;
