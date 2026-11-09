import $ from 'jquery';
import { buildValidations } from 'ember-cp-validations';

import DS from 'ember-data';
import { validator } from 'ember-cp-validations';
import { attr } from 'ember-flexberry-data/utils/attributes';

import DictionaryModel from '@timepath/tmobis-core/models/dictionary';
import { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/dictionary';

let ValidationRules = {
  name: { 
    validators: [
      validator('ds-error'),
      validator('presence', 
      {
        presence: true,
        descriptionKey: 'models.organization-type.validations.name.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale'],
});

let Model = DictionaryModel.extend( Validations, {
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  shortName: DS.attr('string'),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Contacts',
  _parentModelName: 'timepath-core-dictionary',
});

Model.defineProjection('AuditView', 'organization-type', {
  code: attr('Код', { index: 0 }),
  name: attr('Наименование', { index: 1 }),
  actuality: attr('Актуальность', { index: 2, hidden: true }),
  shortName: attr('Короткое наименование', { index: 3 }),
});

Model.defineProjection('OrganizationTypeE', 'organization-type', {
  code: attr('Код', { index: 0 }),
  name: attr('Наименование', { index: 1 }),
  actuality: attr('Актуальность', { index: 2, hidden: true }),
  shortName: attr('Короткое наименование', { index: 3 }),
});

Model.defineProjection('OrganizationTypeL', 'organization-type', {
  code: attr('Код', { index: 0, hidden: true }),
  name: attr('Наименование', { index: 1 }),
  shortName: attr('Короткое наименование', { index: 2 }),
  actuality: attr('Актуальность', { index: 3, hidden: true  }),
});

Model.defineProjection('OrganizationTypeLookup', 'organization-type', {
  shortName: attr('Короткое наименование', { index: 0 }),
  name: attr('Наименование', { index: 1 }),
});

export default Model;
