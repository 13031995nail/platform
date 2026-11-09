import $ from 'jquery';
import BaseModel, { ValidationRules as ParentValidationRules } from '@timepath/tmobis-core/models/base-model';
import DS from 'ember-data';
import { attr, belongsTo } from 'ember-flexberry-data/utils/attributes';
import { buildValidations, validator } from 'ember-cp-validations';
import { once } from '@ember/runloop';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';

let ValidationRules = {
  assignee: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.base-act.validations.inspector.__caption__',
      }),
    ],
  },
  actType: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.base-act.validations.actType.__caption__',
      }),
    ],
  },
  actStatus: {
    validators: [
      validator('ds-error'),
      validator('presence', {
        presence: true,
        descriptionKey: 'models.base-act.validations.actStatus.__caption__',
      }),
    ],
  },
};

const Validations = buildValidations($.extend({}, ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale']
});
const Model = BaseModel.extend(Validations, {
  regDate: DS.attr('date'),
  actNumber: DS.attr('string'),
  changeStatusDate: DS.attr('date'),
  organizationName: DS.attr('string'),
  organizationPhone: DS.attr('string'),
  organizationAddress: DS.attr('string'),
  actNumberIndex: DS.attr('number'),
  actType: DS.belongsTo('act-type', { inverse: null, async: false }),
  assignee: DS.belongsTo('employee', { inverse: null, async: false }),
  actStatus: DS.belongsTo('act-status', { inverse: null, async: false }),
  organization: DS.belongsTo('organization', { inverse: null, async: false }),

  /**
 * Изменение статуса акта.
 */
  onActStatusChanged: function () {
    once(this, 'actStatusChanged');
  }.observes('actStatus'),

  actStatusChanged() {
    if (Object.keys(this.changedBelongsTo()).some(function (item) { return item === 'actStatus'; })) {
      if (this.get('actStatus.id') !== ActStatusKeys.editing) {
        this.set('changeStatusDate', new Date());
      }
    }
  },
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Process.Documents.Flexberry',
});

Model.defineProjection('BaseActE', 'base-act', {
  regDate: attr('', { index: 0 }),
  actNumber: attr('', { index: 2 }),
  changeStatusDate: attr('', { index: 3 }),
  organizationName: attr('', { index: 4 }),
  organizationPhone: attr('', { index: 5 }),
  organizationAddress: attr('', { index: 6 }),
  actNumberIndex: attr('', { index: 7 }),
  // awp: belongsTo('awp', '', {}, { index: 8 }),
  actType: belongsTo('act-type', '', {
    name: attr('', { index: 10 }),
  }, { index: 9 }),
  actStatus: belongsTo('act-status', '', {
    name: attr('', { index: 10 }),
  }, { index: 9 }),
  assignee: belongsTo('employee', '', {
    firstName: attr('', { index: 12 }),
    lastName: attr('', { index: 13 }),
    patronymic: attr('', { index: 14 }),
  }, { index: 11 }),
  organization: belongsTo('organization', '', {
    name: attr('', { index: 16 }),
    organizationType: belongsTo('organization-type', '', {
      name: attr('', { index: 18 })
    }, { index: 17 }),
  }, { index: 15 }),
});

export default Model;
