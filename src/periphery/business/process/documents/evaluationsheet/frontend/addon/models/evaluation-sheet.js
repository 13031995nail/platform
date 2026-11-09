import $ from 'jquery';
import BaseActModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-business-process-acts-base-act/models/base-act';
import DS from 'ember-data';
import { attr, belongsTo, hasMany } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import LevelReadinessEnum from '../enums/level-readiness';

import { computed } from '@ember/object';

export const ActTypeEvaluationSheetKey = '2653b090-817a-4f29-9336-b95159e8878e';

let ValidationRules = {
  awp: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.evaluation-sheet.validations.awp.__caption__',
      }),
    ]
  },
  organization: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.evaluation-sheet.validations.organization.__caption__',
      }),
    ],
  },
  workObject: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.evaluation-sheet.validations.workObject.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});

const Model = BaseActModel.extend(Validations, {
  reportInfo: computed('id', function () {
    const info = { name: 'EvaluationSheet', caption: 'Оценочный лист' };
    info.params = { 'pk': this.id };
    return info;
  }),

  workObject: DS.belongsTo('work-object', { inverse: null, async: false }),
  awp: DS.belongsTo('awp', { inverse: null, async: false }),
  representative: DS.attr('string'),
  objectReadinessIndex: DS.attr('decimal', { defaultValue: 0 }),
  objectReadinessLevel: DS.attr('level-readiness', { defaultValue: LevelReadinessEnum.NotReady }),
  isVerifiedTSO: DS.attr('boolean'),
  ownerlessObjects: DS.attr('boolean'),
  requirements: DS.hasMany('evaluation-sheet-requirement', {
    inverse: 'evaluationSheet',
    async: false
  }),
  actSigns: DS.hasMany('evaluation-sheet-sign', { inverse: 'act', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents'
});

Model.defineProjection('EvaluationSheetL', 'evaluation-sheet', {
  actNumber: attr('Номер', { index: 0 }),
  regDate: attr('Дата оформления', { index: 1 }),
  actStatus: belongsTo('act-status', '', {
    name: attr('Статус', { index: 2 }),
    deleteProtected: attr('', { hidden: true }),
  }, { hidden: true }),
  organization: belongsTo('organization', 'Контрагент', {
    name: attr('Контрагент', { index: 3 })
  }, { index: 3, hidden: true }),
  workObject: belongsTo('work-object', 'Объект', {
    name: attr('Объект', { index: 4 })
  }, { index: 4, hidden: true }),
  awp: belongsTo('awp', 'ОЗП', {
    name: attr('ОЗП', { index: 5, hidden: true })
  }, { index: 5, hidden: true }),
  objectReadinessLevel: attr('Уровень готовности', { index: 6 }),
  actType: belongsTo('act-type', '', {}, { index: 9, hidden: true }),
  objectReadinessIndex: attr('Индекс готовности (число)', { index: 11, hidden: true }),
});

Model.defineProjection('EvaluationSheetE', 'evaluation-sheet', {
  regDate: attr('Дата оформления', { index: 0 }),
  actNumber: attr('Номер документа', { index: 1 }),
  organization: belongsTo('organization', '', {
    name: attr('Контрагент', { index: 2 }),    
    organizationType: belongsTo('organization-type', '', {
      name: attr('', { index: 3, hidden: true })
    }, { index: 17, hidden: true }),
    hierarchy: belongsTo('organization', '', {
    }),
  }, { index: 2 }),
  workObject: belongsTo('work-object', '', {
    name: attr('Объект', { index: 3 })
  }, { index: 3 }),
  awp: belongsTo('awp', '', {
    dateBegin: attr('', { index: 0 }),
    dateEnd: attr('', { index: 1 }),
    name: attr('', { index: 2 }),
  }, { index: 4 , hidden: true }),
  representative: attr('Представитель', { index: 5 }),
  actStatus: belongsTo('act-status', '', {
    name: attr('Статус', { index: 6 })
  }, { index: 6 }),
  actType: belongsTo('act-type', '', {
    name: attr('', { index: 10 }),
  }, { index: 9, hidden: true }),
  objectReadinessLevel: attr('Уровень готовности', { index: 7 }),
  requirements: hasMany('evaluation-sheet-requirement', '', {
    isVerified: attr('Проверено', { index: 0 }),
    reqOrder: attr('Порядок проверки', { index: 1, hidden: true }),
    reqNumber: attr('Номер требования', { index: 2 }),
    shortReqName: attr('Требование', { index: 3 }),
    verificationRequirement: belongsTo('verification-requierement', 'Требование', {
      name: attr('Требование', { hidden: true }),
      code: attr('', { index: 13, hidden: true }),
    }, { hidden: true }),
    supportingDocName: attr('Подтверждающий документ', { index: 4 }),
    supportingDocFile: attr('Приложение', { index: 5 }),
    indicatorName: attr('Показатель', { index: 6 }),
    sortOrder: attr('SortOrder', { index: 8, hidden: true }),
    calculatedReq: attr('Значение', { index: 7 }),
    hasEnteredValue: attr('Значение введено', { hidden: true }),
    isCompletedReq: attr('Замечание устранено', { index: 14 }),
    problemComment: attr('Замечание', { index: 9 }),
    deadline: attr('Срок исправления', { index: 10 }),
    isOptional: attr('Наличие приложения', { index: 11, hidden: true }),
    isCalculated: attr('Рассчитано', { index: 12, hidden: true }),
  }, { index: 8 }),
  assignee: belongsTo('employee', '', {
    firstName: attr('', { index: 12 }),
    lastName: attr('', { index: 13 }),
    patronymic: attr('', { index: 14 }),
    fio: attr('', { index: 2 }),
  }, { index: 11, hidden: true }),
  objectReadinessIndex: attr('Индекс готовности', { index: 15 }),
  isVerifiedTSO: attr('Проверено ТСО', { index: 16 }),
  actSigns: hasMany('evaluation-sheet-sign', '', {
    signDate: attr('', { index: 0 }),
    signDateString: attr('', { index: 0 }),
    signature: attr('', { index: 1 }),
    fio: attr('', { index: 2 }),
    address: attr('', { index: 3 }),
    email: attr('', { index: 4 }),
    phone: attr('', { index: 5 }),
    passport: attr('', { index: 6 }),
    partnerName: attr('', { index: 7 }),
    positionName: attr('', { index: 8 }),
    signType: belongsTo('sign-type', '', {
      name: attr('', { index: 10 }),
    }, { index: 9 }),
    signRole: belongsTo('sign-role', '', {
      name: attr('', { index: 12 }),
    }, { index: 11 }),
    employee: belongsTo('employee', '', {
      firstName: attr('', { index: 14 }),
      lastName: attr('', { index: 15 }),
      patronymic: attr('', { index: 16 }),
    }, { index: 13 }),
    act: belongsTo('evaluation-sheet', '', {
    }, { index: 15, hidden: true }),
  }),
  ownerlessObjects: attr('', { index: 18 }),
});

export default Model;
