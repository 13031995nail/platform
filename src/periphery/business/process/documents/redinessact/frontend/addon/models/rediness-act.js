import $ from 'jquery';
import BaseActModel, { ValidationRules as ParentValidationRules }
  from '@timepath/tmobis-business-process-acts-base-act/models/base-act';
import { attr, belongsTo, hasMany } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import DS from 'ember-data';
import { computed } from '@ember/object';

export const ActTypeReadinessActKey = '22e064cf-1d25-4fbe-b192-d969fb15725b';

let ValidationRules = {
  awp: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.rediness-act.validations.awp.__caption__',
      }),
    ],
  },
  awpAssessingProgram: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.rediness-act.validations.awpAssessingProgram.__caption__',
      }),
    ],
  },
  assignee: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.rediness-act.validations.assignee.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations(
  $.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});

const Model = BaseActModel.extend(Validations, {
  reportInfo: computed('id', function () {
    const info = { name: 'RedinessAct', caption: 'Акт оценки обеспечения готовности к отопительному периоду' };
    info.params = { 'pk': this.id };
    return info;
  }),

  avgObjectReadinessIndex: DS.attr('decimal', { defaultValue: 0 }),
  partnerReadiness: DS.attr('partner-readiness-level'),
  awp: DS.belongsTo('awp', { inverse: null, async: false }),
  awpAssessingProgram: DS.belongsTo('awp-assessing-program', { inverse: null, async: false }),
  objects: DS.hasMany('rediness-act-object', { inverse: 'act', async: false }),
  actSigns: DS.hasMany('readiness-act-sign', { inverse: 'act', async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents',
});

Model.defineProjection('AuditView', 'rediness-act', {
  regDate: attr('', { index: 0 }),
  actNumber: attr('', { index: 2 }),
  changeStatusDate: attr('', { index: 3 }),
  organizationName: attr('', { index: 4 }),
  organizationPhone: attr('', { index: 5 }),
  organizationAddress: attr('', { index: 6 }),
  actNumberIndex: attr('', { index: 7 }),
  awp: belongsTo('awp', '', {}, { index: 9 }),
  awpAssessingProgram: belongsTo('awp-assessing-program', '', {}, { index: 15 }),
  actType: belongsTo('act-type', '', {}, { index: 9 }),
  actStatus: belongsTo('act-status', '', {}, { index: 9 }),
  assignee: belongsTo('employee', '', {}, { index: 11 }),
  organization: belongsTo('organization', '', {}, { index: 15 }),
});
Model.defineProjection('RedinessActL', 'rediness-act', {
  actNumber: attr('', { index: 0 }),
  regDate: attr('', { index: 1 }),
  actStatus: belongsTo('act-status', '', {
    name: attr('Статус', { index: 3 }),
    deleteProtected: attr('', { hidden: true }),
  }, { index: 2, hidden: true }),
  organization: belongsTo('organization', '', {
    name: attr('', { index: 5 }),
    organizationType: belongsTo('organization-type', '', {
      name: attr('', { index: 7, hidden: true })
    }, { index: 6, hidden: true }),
  }, { index: 4, hidden: true }),
  partnerReadiness: attr('', { index: 8 }),
  awp: belongsTo('awp', '', {}, { index: 9, hidden: true }),
  actType: belongsTo('act-type', '', {
    name: attr('', { index: 11, hidden: true }),
  }, { index: 10, hidden: true }),
  
});
Model.defineProjection('RedinessActE', 'rediness-act', {
  regDate: attr('', { index: 0 }),
  actNumber: attr('', { index: 2 }),
  changeStatusDate: attr('', { index: 3 }),
  organizationName: attr('', { index: 4 }),
  organizationPhone: attr('', { index: 5 }),
  organizationAddress: attr('', { index: 6 }),
  actNumberIndex: attr('', { index: 7 }),
  avgObjectReadinessIndex: attr('', { index: 8 }),
  partnerReadiness: attr('', { index: 8 }),
  awp: belongsTo('awp', '', {
    dateBegin: attr('', { index: 0 }),
    dateEnd: attr('', { index: 1 }),
    isCurrent: attr('', { index: 2 }),
    actuality: attr('', { index: 3, hidden: true }),
    name: attr('', { index: 4, hidden: true }),
  }, { index: 8, hidden: true }),
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
    fio: attr('', { index: 15 }),
  }, { index: 11, hidden: true }),
  organization: belongsTo('organization', '', {
    name: attr('', { index: 16 }),
    organizationType: belongsTo('organization-type', '', {
      name: attr('', { index: 18 })
    }, { index: 17, hidden: true }),
    hierarchy: belongsTo('organization', '', {
      name: attr('', { hidden: true })
    }, { hidden: true }),
  }, { index: 15, hidden: true }),
  awpAssessingProgram: belongsTo('awp-assessing-program', '', {
    actNumber: attr('', { index: 16 }),
    resolutionNumber: attr('', { index: 16 }),
  }, { index: 15 }),
  objects: hasMany('rediness-act-object', '', {
    number: attr('Номер', { index: 0 }),
    workObject: belongsTo('work-object', 'Объект', {
      name: attr('Наименование', { index: 2 }),
    }, { index: 1, hidden: true }),
    objectReadiness: attr('Уровень готовности объекта', { index: 3 }),
    act: belongsTo('rediness-act', '', {
    }, { index: 4, hidden: true }),
  }),
  actSigns: hasMany('readiness-act-sign', '', {
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
    act: belongsTo('rediness-act', '', {
    }, { index: 15, hidden: true }),
  })
});

export default Model;
