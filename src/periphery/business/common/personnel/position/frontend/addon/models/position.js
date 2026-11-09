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
      validator('presence', {
        presence: true,
        descriptionKey: 'models.position.validations.name.__caption__'
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
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Personnel',
  _parentModelName: 'timepath-core-dictionary'
});

Model.defineProjection('AuditView', 'position', {
  code: attr('Код', { index: 0 }),
  name: attr('Наименование', { index: 1 }),
  actuality: attr('Актуальность', { index: 2, hidden: true }),
});

Model.defineProjection('PositionE', 'position', {
  code: attr('Код', { index: 0 }),
  name: attr('Наименование', { index: 1 }),
  actuality: attr('Актуальность', { index: 2, hidden: true }),
});

Model.defineProjection('PositionL', 'position', {
  code: attr('Код', { index: 0, hidden: true }),
  name: attr('Наименование', { index: 1 }),
  actuality: attr('Актуальность', { index: 2, hidden: true  }),
});
export default Model;
