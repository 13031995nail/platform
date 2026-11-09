import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-core/models/base-model';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import DS from 'ember-data';

let ValidationRules = {
  act: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.rediness-act-object.validations.act.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  number: DS.attr('number'),
  objectReadiness: DS.attr('level-readiness'),
  workObject: DS.belongsTo('work-object', { inverse: null, async: false }),
  act: DS.belongsTo('rediness-act', { inverse: 'objects', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'rediness-act-object', {
  number: attr(''),
  objectReadiness: attr(''),
  workObject: belongsTo('work-object', '', {}, { index: 11 }),
  act: belongsTo('rediness-act', '', {}, { index: 15 }),
});
Model.defineProjection('RedinessActObjectD', 'rediness-act-object', {
  number: attr(''),
  objectReadiness: attr(''),
  workObject: belongsTo('work-object', '', {
    name: attr('Наименование')
  }, { index: 15, hidden: true }),
  act: belongsTo('rediness-act', '', {
  }, { index: 15, hidden: true }),
});

export default Model;
