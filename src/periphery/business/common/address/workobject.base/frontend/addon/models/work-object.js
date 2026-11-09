import $ from 'jquery';
import ActualEntityModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/actual-entity';
import DS from 'ember-data';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { validator, buildValidations } from 'ember-cp-validations';

let ValidationRules = {
  name: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.work-object.validations.name.__caption__',
      }),
    ],
  },
  addressString: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.work-object.validations.addressString.__caption__',
      }),
    ],
  },
  objectFunction: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.work-object.validations.objectFunction.__caption__',
      }),
    ],
  },
  organization: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.work-object.validations.organization.__caption__',
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
  addressString: DS.attr('string'),
  objectFunction: DS.belongsTo('object-function', { inverse: null, async: false }),
  organization: DS.belongsTo('organization', { inverse: null, async: false }),
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Address',
  _parentModelName: 'actual-object'
});

Model.defineProjection('AuditView', 'work-object', {
  name: attr(''),
  actuality: attr(''),
  addressString: attr(''),
  objectFunction: belongsTo('object-function', '', {}),
  organization: belongsTo('organization', '', {})
});
Model.defineProjection('WorkObjectE', 'work-object', {
  name: attr('Наименование'),
  actuality: attr('Актуально'),
  addressString: attr('Адрес '),
  objectFunction: belongsTo('object-function', '', {
    name: attr('Наименование')
  }),
  organization: belongsTo('organization', '', {
    name: attr('Наименование')
  })
});
Model.defineProjection('WorkObjectL', 'work-object', {
  name: attr('Наименование объекта', { index: 0 }),
  addressString: attr('Адрес', { index: 1 }),
  objectFunction: belongsTo('object-function', '', {
    name: attr('Назначение', { index: 2 })
  }, { hidden: true }),
  organization: belongsTo('organization', '', {
    name: attr('Контрагент', { index: 3 })
  }, { hidden: true })
});

export default Model;