import $ from 'jquery';
import ActualEntityModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/actual-entity';
import DS from 'ember-data';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { validator, buildValidations } from 'ember-cp-validations';
import { computed } from '@ember/object';
import OrganizationTypeEnum from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';
import { isEmpty } from '@ember/utils';

let ValidationRules = {
  name: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.organization.validations.name.__caption__',
      }),
    ],
  },
  organizationType: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.organization.validations.organizationType.__caption__',
      }),
    ],
  },
  acronym: {
    disabled: computed('model.organizationType', function () {
      return this.get('model.organizationType.id') !== OrganizationTypeEnum.Municipality;
    }),
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.organization.validations.acronym.__caption__',
      }),
    ],
  },
  hierarchy: {
    disabled: computed('model.organizationType', function () {
      return isEmpty(this.get('model.organizationType')) || this.get('model.organizationType.id') === OrganizationTypeEnum.Municipality;
    }),
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.organization.validations.hierarchy.__caption__',
      }),
    ],
  }
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = ActualEntityModel.extend(Validations, {
  name: DS.attr('string'),
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
  actuality: DS.attr('boolean'),
  address: DS.attr('string'),
  legalAddress: DS.attr('string'),
  phone: DS.attr('string'),
  email: DS.attr('string'),
  acronym: DS.attr('string'),
  town: DS.belongsTo('town', { inverse: null, async: false }),
  organizationType: DS.belongsTo('organization-type', { inverse: null, async: false }),
  hierarchy: DS.belongsTo('organization', { inverse: null, async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Contacts',
  _parentModelName: 'actual-object'
});

Model.defineProjection('AuditView', 'organization', {
  name: attr(''),
  actuality: attr(''),
  address: attr(''),
  legalAddress: attr(''),
  phone: attr(''),
  email: attr(''),
  acronym: attr(''),
  organizationType: belongsTo('organization-type', '', {}),
  hierarchy: belongsTo('organization', '', {}),
});
Model.defineProjection('OrganizationE', 'organization', {
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
});
Model.defineProjection('OrganizationL', 'organization', {
  name: attr('Наименование', { index: 0 }),
  legalAddress: attr('Юридический адрес', { index: 1 }),
  organizationType: belongsTo('organization-type', '', {
    name: attr('Тип контрагента', { index: 2 })
  }, { hidden: true }),
  phone: attr('Номер телефона', { index: 3 }),
  actuality: attr('Актуально', { hidden: true }),
});
Model.defineProjection('OrganizationName', 'organization', {
  name: attr('Наименование', { index: 0 }),
  organizationType: belongsTo('organization-type', '', {
  }, { hidden: true }),
  actuality: attr('Актуально', { hidden: true }),
  hierarchy: belongsTo('organization', '', {
    name: attr('Муниципальное образование', { hidden: true })
  }, { hidden: true }),
});

export default Model;
