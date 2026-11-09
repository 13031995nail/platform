import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/base-model';
import DS from 'ember-data';
import { attr, belongsTo, hasMany } from 'ember-flexberry-data/utils/attributes';
import { validator, buildValidations } from 'ember-cp-validations';
import AdmissionCommitteeStatusEnum from '../enums/admission-committee-status';

let ValidationRules = {
  awp: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.admission-committee.validations.awp.__caption__',
      }),
    ],
  },
  organization: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.admission-committee.validations.organization.__caption__',
      }),
    ],
  }
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  approvedDate: DS.attr('date'),
  regDate: DS.attr('date'),
  status: DS.attr('admission-committee-status', { defaultValue: AdmissionCommitteeStatusEnum.Editing }),
  leader: DS.belongsTo('admission-committee-employee', { inverse: null, async: false }),
  deputyLeader: DS.belongsTo('admission-committee-employee', { inverse: null, async: false }),
  awp: DS.belongsTo('awp', { inverse: null, async: false }),
  organization: DS.belongsTo('organization', { inverse: null, async: false }),
  admissionCommitteeEmployee: DS.hasMany('admission-committee-employee', { inverse: 'admissionCommittee', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Personnel',
});

Model.defineProjection('AuditView', 'admission-committee', {
  status: attr(''),
  approvedDate: attr(''),
  regDate: attr(''),
  leader: belongsTo('admission-committee-employee', '', {}, {}),
  deputyLeader: belongsTo('admission-committee-employee', '', {}, {}),
  awp: belongsTo('awp', '', {}, {}),
  organization: belongsTo('organization', '', {}, {}),
  admissionCommitteeEmployee: hasMany('admission-committee-employee', '', {}),
});
Model.defineProjection('AdmissionCommitteeE', 'admission-committee', {
  status: attr('Статус'),
  regDate: attr('Дата формирования'),
  approvedDate: attr('Дата утверждения'),
  organization: belongsTo('organization', '', {
    name: attr('Наименование'),
    actuality: attr('Актуально'),
    address: attr('Адрес '),
    legalAddress: attr('адрес'),
    phone: attr('Номер телефона'),
    email: attr('E-mail'),
    acronym: attr('Акроним'),
    organizationType: belongsTo('organization-type', '', {
      name: attr('Тип контрагента')
    }),
    hierarchy: belongsTo('organization', '', {
      name: attr('Муниципальное образование')
    }),
    town: belongsTo('town', '', {}),
  }),
  awp: belongsTo('awp', '', {
    dateBegin: attr('Дата начала'),
    dateEnd: attr('Дата конца'),
    isCurrent: attr('Текущий'),
    actuality: attr('Актуальность'),
    name: attr(''),
  }),
  leader: belongsTo('admission-committee-employee', '', {
    employee: belongsTo('ФИО', '', {
      fio: attr('', { hidden: true }),
      lastName: attr('', { hidden: true }),
      firstName: attr('', { hidden: true }),
      patronymic: attr('', { hidden: true }),
    }, { index: 0 })
  }, {}),
  deputyLeader: belongsTo('admission-committee-employee', '', {
    employee: belongsTo('ФИО', '', {
      fio: attr('', { hidden: true }),
      lastName: attr('', { hidden: true }),
      firstName: attr('', { hidden: true }),
      patronymic: attr('', { hidden: true }),
    }, { index: 0 })
  }, {}),
  admissionCommitteeEmployee: hasMany('admission-committee-employee', '', {
    employee: belongsTo('ФИО', '', {
      fio: attr('', { hidden: true }),
      lastName: attr('', { hidden: true }),
      firstName: attr('', { hidden: true }),
      patronymic: attr('', { hidden: true }),
    }, { index: 0 }),
    placesOfWork: attr('Место работы', { index: 1 }),
  }),
});
Model.defineProjection('AdmissionCommitteeL', 'admission-committee', {
  regDate: attr('Дата формирования', { index: 0 }),
  status: attr('Статус', { index: 1 }),
  organization: belongsTo('organization', '', {
    name: attr('Муниципальное образование', { index: 2 })
  }, { hidden: true }),
  leader: belongsTo('admission-committee-employee', '', {
    employee: belongsTo('employee', '', {
      fio: attr('Председатель', { index: 3 }),
      lastName: attr('', { hidden: true }),
      firstName: attr('', { hidden: true }),
      patronymic: attr('', { hidden: true }),
    }, { hidden: true }),
  }, { hidden: true }),
});

export default Model;
