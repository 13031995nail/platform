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
        descriptionKey: 'models.awp-readiness-passport-object.validations.act.__caption__',
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
  workObject: DS.belongsTo('work-object', { inverse: null, async: false }),
  act: DS.belongsTo('awp-readiness-passport', { inverse: 'objects', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'awp-readiness-passport-object', {
  workObject: belongsTo('work-object', '', {}, { index: 11 }),
  act: belongsTo('awp-readiness-passport', '', {}, { index: 15 }),
});
Model.defineProjection('AwpReadinessPassportObjectE', 'awp-readiness-passport-object', {
  workObject: belongsTo('work-object', '', {
    name: attr('Наименование')
  }, { index: 15, hidden: true }),
  act: belongsTo('awp-readiness-passport', '', {
  }, { index: 15, hidden: true }),
});

export default Model;
