import $ from 'jquery';
import BaseActModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-business-process-acts-base-act/models/base-act';
import { attr, belongsTo, hasMany } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import DS from 'ember-data';
import { computed } from '@ember/object';

export const ActTypeProgramKey = '32c1d050-7812-4e8b-9daa-257c09ecaf59';

let ValidationRules = {
  awp: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.awpAssessingProgram.validations.awp.__caption__',
      }),
    ]
  }
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});

const Model = BaseActModel.extend(Validations, {
  reportInfo: computed('id', function () {
    const info = {
      name: 'AwpAssessingProgram',
      caption: 'Программа проведения оценки готовности к отопительному периоду'
    };
    info.params = { 'pk': this.id };
    return info;
  }),

  filesPropertyNames: ['resolutionDoc'],

  resolutionNumber: DS.attr('string'),
  resolutionDate: DS.attr('date'),
  approvedDate: DS.attr('date'),
  resolutionDoc: DS.attr('string'),
  awp: DS.belongsTo('awp', { inverse: null, async: false }),
  committee: DS.belongsTo('admission-committee', { inverse: null, async: false }),
  schedules: DS.hasMany('awp-assessing-program-evaluation-schedule', { inverse: 'program', async: false }),
  objects: DS.hasMany('awp-assessing-program-object', { inverse: 'program', async: false }),
  actSigns: DS.hasMany('awp-assessing-program-sign', { inverse: 'act', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'awp-assessing-program', {
  regDate: attr('', { index: 0 }),
  resolutionNumber: attr('', { index: 0 }),
  resolutionDate: attr('', { index: 0 }),
  approvedDate: attr('', { index: 0 }),
  resolutionDoc: attr('', { index: 0 }),
  actNumber: attr('', { index: 2 }),
  changeStatusDate: attr('', { index: 3 }),
  organizationName: attr('', { index: 4 }),
  organizationPhone: attr('', { index: 5 }),
  organizationAddress: attr('', { index: 6 }),
  actNumberIndex: attr('', { index: 7 }),
  awp: belongsTo('awp', '', {}, { index: 9 }),
  actType: belongsTo('act-type', '', {}, { index: 9 }),
  actStatus: belongsTo('act-status', '', {}, { index: 9 }),
  assignee: belongsTo('employee', '', {}, { index: 11 }),
  organization: belongsTo('organization', '', {}, { index: 15 }),
  committee: belongsTo('employee', '', {
  }, { index: 11, hidden: true }),
});
Model.defineProjection('AwpAssessingProgramL', 'awp-assessing-program', {
  actNumber: attr('Номер', { index: 0 }),
  regDate: attr('Дата оформления', { index: 1 }),
  actStatus: belongsTo('act-status', '', {
    name: attr('Статус', { index: 2 }),
    deleteProtected: attr('', { hidden: true }),
  }, { hidden: true }),
  approvedDate: attr('Дата утверждения', { index: 3 }),
  resolutionNumber: attr('', { hidden: true }),
  assignee: belongsTo('employee', '', {
    fio: attr('Председатель', { index: 3 }),
    firstName: attr('', { hidden: true }),
    lastName: attr('', { hidden: true }),
    patronymic: attr('', { hidden: true }),
  }, { index: 11, hidden: true }),
  organization: belongsTo('organization', '', {
    name: attr('Муниципальное образование', { index: 3 }),
  }, { index: 15, hidden: true }),
});
Model.defineProjection('AwpAssessingProgramE', 'awp-assessing-program', {
  regDate: attr('', { index: 0 }),
  actNumber: attr('', { index: 2 }),
  resolutionNumber: attr('', { index: 3 }),
  resolutionDate: attr('', { index: 3 }),
  approvedDate: attr('', { index: 3 }),
  resolutionDoc: attr('', { index: 3 }),
  changeStatusDate: attr('', { index: 3 }),
  organizationName: attr('', { index: 4 }),
  organizationPhone: attr('', { index: 5 }),
  organizationAddress: attr('', { index: 6 }),
  actNumberIndex: attr('', { index: 7 }),
  awp: belongsTo('awp', '', {
    dateBegin: attr('', { index: 0 }),
    dateEnd: attr('', { index: 1 }),
    isCurrent: attr('', { index: 2 }),
    actuality: attr('', { index: 3, hidden: true }),
    name: attr('', { index: 4, hidden: true }),
  }, { index: 8 , hidden: true }),
  actType: belongsTo('act-type', '', {
    name: attr('', { index: 10 }),
  }, { index: 9, hidden: true }),
  actStatus: belongsTo('act-status', '', {
    name: attr('', { index: 10 }),
  }, { index: 9, hidden: true }),
  assignee: belongsTo('employee', '', {
    firstName: attr('', { index: 12 }),
    lastName: attr('', { index: 13 }),
    patronymic: attr('', { index: 14 }),
    fio: attr('', { index: 2 }),
  }, { index: 11, hidden: true }),
  committee: belongsTo('employee', '', {
  }, { index: 11, hidden: true }),
  organization: belongsTo('organization', '', {
    name: attr('', { index: 16 }),
    organizationType: belongsTo('organization-type', '', {
      name: attr('', { index: 18, hidden: true })
    }, { index: 17, hidden: true }),
    hierarchy: belongsTo('organization', '', {
      name: attr('', { hidden: true })
    }, { hidden: true }),
  }, { index: 15, hidden: true }),
  schedules: hasMany('awp-assessing-program-evaluation-schedule', '', {
    objectFunction: belongsTo('object-function', 'Назначение объектов', {
      name: attr('', { index: 18, hidden: true })
    }, { index: 0 }),
    objectsCount: attr('Количество объектов', { index: 1 }),
    startDate: attr('Дата начала проверки', { index: 2 }),
    finishDate: attr('Дата окончания проверки', { index: 3 }),
    program: belongsTo('awp-assessing-program', '', {}, { index: 9, hidden: true }),
  }),
  objects: hasMany('awp-assessing-program-object', '', {
    workObject: belongsTo('work-object', 'Объект', {
      name: attr('Наименование', { index: 2, hidden: true }),
    }, { index: 15 }),
    program: belongsTo('awp-assessing-program', '', {
    }, { index: 15, hidden: true }),
  }),
  actSigns: hasMany('awp-assessing-program-sign', '', {
    signDate: attr('', { index: 0 }),
    signDateString: attr('', { index: 0 }),
    signature: attr('', { index: 1 }),
    fio: attr('', { index: 2 }),
    address: attr('', { index: 3 }),
    email: attr('', { index: 4 }),
    phone: attr('', { index: 5 }),
    passport: attr('', { index: 6 }),
    partnerName: attr('', { index: 7 }),
    positionName: attr('', { index: 8 }),
    signType: belongsTo('sign-type', '', {
      name: attr('', { index: 10 }),
    }, { index: 9 }),
    signRole: belongsTo('sign-role', '', {
      name: attr('', { index: 12 }),
    }, { index: 11 }),
    employee: belongsTo('employee', '', {
      firstName: attr('', { index: 14 }),
      lastName: attr('', { index: 15 }),
      patronymic: attr('', { index: 16 }),
    }, { index: 13 }),
    act: belongsTo('awp-assessing-program', '', {
    }, { index: 15, hidden: true }),
  })
});

export default Model;
