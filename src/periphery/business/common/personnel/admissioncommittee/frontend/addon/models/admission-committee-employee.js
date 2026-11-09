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
  placesOfWork: DS.attr('string'),
  employee: DS.belongsTo('employee', { inverse: null, async: false }),
  placeOfEmployment: DS.belongsTo('place-of-employment', { inverse: null, async: false }),
  admissionCommittee: DS.belongsTo('admission-committee', { inverse: 'admissionCommitteeEmployee', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Personnel',
});

Model.defineProjection('AdmissionCommitteeEmployeeD', 'admission-committee-employee', {
  employee: belongsTo('employee', 'ФИО', {}, { index: 1, hidden: true }),
  placeOfEmployment: belongsTo('place-of-employment', 'ФИО', {}, { index: 2 }),
  placesOfWork: attr('Место работы', { index: 3 }),
  admissionCommittee: belongsTo('admission-committee', '', {}, { hidden: true }),
});

export default Model;