import $ from 'jquery';
import ActSignModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-business-process-acts-actsign/models/act-sign';
import { buildValidations, validator } from 'ember-cp-validations';
import DS from 'ember-data';

let ValidationRules = {
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = ActSignModel.extend(Validations, {
  act: DS.belongsTo('evaluation-sheet', { inverse: 'actSigns', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

export default Model;
