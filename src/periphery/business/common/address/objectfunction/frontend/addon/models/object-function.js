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
        descriptionKey: 'models.object-function.validations.name.__caption__',
      }),
    ],
  }
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = DictionaryModel.extend(Validations, {
  name: DS.attr('string'),
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  actuality: DS.attr('boolean')
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Address',
  _parentModelName: 'timepath-core-dictionary'
});

Model.defineProjection('AuditView', 'object-function', {
  name: attr('Name'),
  actuality: attr('Actuality')
});
Model.defineProjection('ObjectFunctionE', 'object-function', {
  name: attr('Наименование'),
  actuality: attr('Актуально')
});
Model.defineProjection('ObjectFunctionL', 'object-function', {
  name: attr('Наименование'),
  actuality: attr('', { hidden: true })
});

export default Model;
