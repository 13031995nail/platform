import $ from 'jquery';
import DictionaryModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/dictionary';
import DS from 'ember-data';
import { attr } from 'ember-flexberry-data/utils/attributes';
import { validator, buildValidations } from 'ember-cp-validations';

let ValidationRules = {
  name: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.act-type.validations.name.__caption__',
      }),
    ],
  },
  acronym: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.act-type.validations.acronym.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = DictionaryModel.extend(Validations, {
  acronym: DS.attr('string'),
  name: DS.attr('string'),
  shortName: DS.attr('string'),
  editFormName: DS.attr('string'),
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  actuality: DS.attr('boolean')
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents.Flexberry',
  _parentModelName: 'timepath-core-dictionary'
});

Model.defineProjection('AuditView', 'act-type', {
  name: attr('', { index: 0 }),
  shortName: attr('', { index: 1 }),
  editFormName: attr('', { index: 2 }),
  acronym: attr('', { index: 3 }),
  actuality: attr('', { index: 4 }),
});
Model.defineProjection('ActTypeE', 'act-type', {
  name: attr('Наименование', { index: 0 }),
  shortName: attr('Короткое наименование', { index: 1 }),
  editFormName: attr('Наименование формы редактирования', { index: 2 }),
  acronym: attr('', { index: 3 }),
  actuality: attr('Актуально', { index: 4 }),
});
Model.defineProjection('ActTypeL', 'act-type', {
  name: attr('Наименование', { index: 0 }),
  acronym: attr('', { index: 1 }),
  actuality: attr('', { index: 2, hidden: true })
});

export default Model;
