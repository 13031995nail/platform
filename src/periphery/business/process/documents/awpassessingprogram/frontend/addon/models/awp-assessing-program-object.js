import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-core/models/base-model';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import DS from 'ember-data';

let ValidationRules = {
  program: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awp-assessing-program-object.validations.program.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  workObject: DS.belongsTo('work-object', { inverse: null, async: false }),
  program: DS.belongsTo('awp-assessing-program', { inverse: 'objects', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'awp-assessing-program-object', {
  workObject: belongsTo('work-object', '', {}, { index: 11 }),
  program: belongsTo('awp-assessing-program', '', {}, { index: 15 }),
});
Model.defineProjection('ProgramObjectD', 'awp-assessing-program-object', {
  workObject: belongsTo('work-object', '', {
    name: attr('Наименование')
  }, { index: 15, hidden: true }),
  program: belongsTo('awp-assessing-program', '', {
  }, { index: 15, hidden: true }),
});

export default Model;