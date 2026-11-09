import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/base-model';
import DS from 'ember-data';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import { computed, observer } from '@ember/object';
import Moment from 'moment';
import config from 'ember-get-config';
import { isBlank } from '@ember/utils';
import SignRoleEnum from '../enums/sign-role';

let ValidationRules = {
  signRole: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.act-sign.validations.signRole.__caption__',
      }),
    ],
  },
  act: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.act-sign.validations.act.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  signDate: DS.attr('date'),
  signDateString: DS.attr('string'),
  signature: DS.attr('string'),
  fio: DS.attr('string'),
  address: DS.attr('string'),
  email: DS.attr('string'),
  phone: DS.attr('string'),
  passport: DS.attr('string'),
  partnerName: DS.attr('string'),
  positionName: DS.attr('string'),
  employee: DS.belongsTo('employee', { inverse: null, async: false }),
  signRole: DS.belongsTo('sign-role', { inverse: null, async: false }),
  signType: DS.belongsTo('sign-type', { inverse: null, async: false }),
  filesPropertyNames: ['signature'],
  roleIndex: computed('signRole', function () {
    return Object.values(SignRoleEnum).indexOf(this.get('signRole.id'));
  }),
  dateChange: observer('signDate', function () {
    if (this.isDeleted) {
      return;
    }

    let date = this.signDate,
      newDateStr = isBlank(date) ? null : new Moment(date).format(config.moment.dateFormat),
      dateString = this.signDateString;

    if (dateString !== newDateStr) {
      this.set('signDateString', newDateStr);
    }
  }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents.Flexberry',
});

Model.defineProjection('ActSignD', 'act-sign', {
  signRole: belongsTo('sign-role', '', {
    name: attr('Роль', { index: 1 }),
  }, { index: 0, hidden: true }),
  fio: attr('Фамилия, имя, отчество', { index: 2 }),
  positionName: attr('Должность', { index: 3 }),
  partnerName: attr('Организация', { index: 4 }),
  signature: attr('Подпись', { index: 5 }),
  signDate: attr('Дата подписания', { index: 6, hidden: true }),
  signDateString: attr('Дата подписания', { index: 6 }),
  signType: belongsTo('sign-type', '', {
    name: attr('', { index: 8, hidden: true }),
  }, { index: 7, hidden: true }),
  employee: belongsTo('employee', 'Сотрудник', {
    firstName: attr('', { index: 10, hidden: true }),
    lastName: attr('', { index: 11, hidden: true }),
    patronymic: attr('', { index: 12, hidden: true }),
  }, { index: 9, hidden: true }),
});

export default Model;
