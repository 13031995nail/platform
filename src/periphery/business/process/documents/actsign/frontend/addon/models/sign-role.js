import $ from 'jquery';
import DictionaryModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/dictionary';
import { attr } from 'ember-flexberry-data/utils/attributes';
import { validator, buildValidations } from 'ember-cp-validations';
import DS from 'ember-data';

let ValidationRules = {
  name: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.sign-role.validations.name.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});

const Model = DictionaryModel.extend(Validations, {
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents.Flexberry',
  _parentModelName: 'timepath-core-dictionary'
});

Model.defineProjection('AuditView', 'sign-role', {
  name: attr('', { index: 0 }),
  actuality: attr('', { index: 4 }),
});
Model.defineProjection('SignRoleE', 'sign-role', {
  name: attr('Наименование', { index: 0 }),
  actuality: attr('Актуально', { index: 4 }),
});
Model.defineProjection('SignRoleL', 'sign-role', {
  name: attr('Наименование', { index: 0 }),
  actuality: attr('', { index: 2, hidden: true })
});

export default Model;
