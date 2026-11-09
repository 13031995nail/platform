import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/base-model';
import DS from 'ember-data';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations } from 'ember-cp-validations';

let ValidationRules = {
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  signature: DS.attr('string'),
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  employee: DS.belongsTo('employee', { inverse: null, async: false }),
  filesPropertyNames: ['signature'],
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Signs',
});

Model.defineProjection('AuditView', 'employee-signature', {
  signature: attr('', { index: 0 }),
  employee: belongsTo('employee', '', {}),
});
Model.defineProjection('EmployeeSignatureE', 'employee-signature', {
  signature: attr('', { index: 0 }),
  employee: belongsTo('employee', '', {}),
});
Model.defineProjection('EmployeeSignatureL', 'employee-signature', {
  signature: attr('', { index: 0 }),
  employee: belongsTo('employee', '', {}),
});

export default Model;
