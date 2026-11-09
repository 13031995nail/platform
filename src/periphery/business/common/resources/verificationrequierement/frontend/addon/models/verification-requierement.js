import $ from 'jquery';
import { buildValidations } from 'ember-cp-validations';

import DS from 'ember-data';
import { validator } from 'ember-cp-validations';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';

import DictionaryModel from '@timepath/tmobis-core/models/dictionary';
import { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/dictionary';

let ValidationRules = {
  name: { 
    validators: [
      validator('ds-error'),
      validator('presence', 
      {
        presence: true,
        descriptionKey: 'models.verification-requierement.validations.name.__caption__',
      }),
    ],
  },
  indicatorName: {
    validators: [
      validator('ds-error'),
      validator('presence', 
      {
        presence: true,
        descriptionKey: 'models.verification-requierement.validations.indicatorName.__caption__',
      }),
    ],
  },
  reqNumber: {
    validators: [
      validator('ds-error'),
      validator('presence', 
      {
        presence: true,
        descriptionKey: 'models.verification-requierement.validations.reqNumber.__caption__',
      }),
    ],
  },
  organizationType: {
    validators: [
      validator('ds-error'),
      validator('presence', 
      {
        presence: true,
        descriptionKey: 'models.verification-requierement.validations.organizationType.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale'],
});

let Model = DictionaryModel.extend(Validations, {
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  isCalculated: DS.attr('boolean'),
  reqNumber: DS.attr('string'),
  shortIndicatorName: DS.attr('string'),
  shortReqName: DS.attr('string'),
  indicatorName: DS.attr('string'),
  supportingDocName: DS.attr('string'),
  weight: DS.attr('decimal'),
  isOptional: DS.attr('boolean'),
  ownerlessObjects: DS.attr('boolean'),
  organizationType: DS.belongsTo('organization-type', { inverse: null, async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Resources',
  _parentModelName: 'timepath-core-dictionary',
});

Model.defineProjection('AuditView', 'verification-requierement', {
  code: attr('code', { index: 0 }),
  name: attr('name', { index: 1 }),
  isCalculated: attr('isCalculated', { index: 2 }),
  organizationType: belongsTo('organization-type', '', {

  }, { index: 3, hidden: true }),
  shortReqName: attr('shortReqName', { index: 4 }),
  reqNumber: attr('reqNumber', { index: 5 }),
  shortIndicatorName: attr('shortIndicatorName', { index: 6 }),
  indicatorName: attr('indicatorName', { index: 7 }),
  supportingDocName: attr('supportingDocName', { index: 8 }),
  weight: attr('weight', { index: 9 }),
  actuality: attr('Актуальность', { index: 10, hidden: true }),
});

Model.defineProjection('VerificationRequierementE', 'verification-requierement', {
  code: attr('Порядок требования', { index: 0 }),
  name: attr('Требование', { index: 1 }),
  isCalculated: attr('Расчетный', { index: 2 }),
  organizationType: belongsTo('organization-type', '', {
    name: attr('Тип контрагента', { index: 4 }),
  }, { index: 3, hidden: true }),
  shortReqName: attr('Номер требования', { index: 5 }),
  reqNumber: attr('Номер проверки', { index: 6 }),
  shortIndicatorName: attr('Короткое название', { index: 7 }),
  indicatorName: attr('Показатель', { index: 8 }),
  supportingDocName: attr('Наименование подтверждающего документа', { index: 9 }),
  weight: attr('Вес', { index: 10 }),
  actuality: attr('Актуальность', { index: 11 }),
  isOptional: attr('Наличие приложения', { index: 12 }),
});

Model.defineProjection('VerificationRequierementL', 'verification-requierement', {
  indicatorName: attr('Показатель', { index: 0 }),
  shortIndicatorName: attr('Короткое название', { index: 1 }),
  name: attr('Требование', { index: 2 }),
  reqNumber: attr('Номер проверки', { index: 3 }),
  weight: attr('Вес', { index: 4 }),
  supportingDocName: attr('Наименование подтверждающего документа', { index: 5 }),
  organizationType: belongsTo('organization-type', '', {
    name: attr('Тип контрагента', { index: 6 }),
  }, { hidden: true }),
  actuality: attr('Актуальность', { index: 7, hidden: true }),
});

export default Model;
