import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/base-model';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import DS from 'ember-data';
import { validator, buildValidations } from 'ember-cp-validations';

let ValidationRules = {
  organization: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.place-of-employment.validations.organization.__caption__',
      }),
    ],
  },
  position: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.place-of-employment.validations.position.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  phone: DS.attr('string'),
  organization: DS.belongsTo('organization', { inverse: null, async: false }),
  position: DS.belongsTo('position', { inverse: null, async: false }),
  employee: DS.belongsTo('employee', { inverse: null, async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Personnel',
});

Model.defineProjection('AuditView', 'place-of-employment', {
  phone: attr(''),
  organization: belongsTo('organization', '', {}),
  position: belongsTo('position', '', {}),
  employee: belongsTo('employee', '', {}),
});
Model.defineProjection('PlaceOfEmploymentD', 'place-of-employment', {
  organization: belongsTo('organization', 'Контрагент', {
    name: attr('', { hidden: true }),
  }),
  position: belongsTo('position', 'Должность ', {
    name: attr('', { hidden: true }),
  }),
  phone: attr('Телефон'),
  employee: belongsTo('employee', '', {
  }, { hidden: true }),
});
Model.defineProjection('EmployeeAdmissionCommitteeL', 'place-of-employment', {
  employee: belongsTo('employee', '', {
    lastName: attr('Фамилия', { index: 2 }),
    firstName: attr('Имя', { index: 3 }),
    patronymic: attr('Отчество', { index: 4 }),
    phone: attr('Номер телефона', { index: 5, hidden: true }),
    email: attr('E-mail', { index: 6, hidden: true }),
    actuality: attr('Актуальность', { index: 7, hidden: true }),
    fio: attr('ФИО', { index: 8, hidden: true }),
  }, { hidden: true, index: 1 }),
  organization: belongsTo('organization', 'Контрагент', {
    name: attr('Место работы', { index: 10 }),
    organizationType: belongsTo('organization-type', '', {
      name: attr('Тип контрагента', { hidden: true })
    }, { hidden: true }),
  }, { hidden: true, index: 9 }),
  position: belongsTo('position', '', {
    name: attr('', { hidden: true }),
  }, { hidden: true }),
});

export default Model;
