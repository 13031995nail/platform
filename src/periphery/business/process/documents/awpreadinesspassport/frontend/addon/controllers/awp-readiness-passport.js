import EditFormController from '@timepath/tmobis-business-process-acts-base-act/controllers/documents-edit-base';

import { computed } from '@ember/object';
import { inject as service } from '@ember/service';
import $ from 'jquery';

import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { ComplexPredicate, SimplePredicate } from 'ember-flexberry-data/query/predicate';

import OrgTypeKeys from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';
import SignRoleEnum from '@timepath/tmobis-business-process-acts-actsign/enums/sign-role';

export default EditFormController.extend({
  reports: service(),
  httpService: service('http-request'),
  
  parentRoute: 'awp-readiness-passports',
  actSignProvider: service('act-sign-provider'),

  init() {
    this._super(...arguments);
    this.reports.bindActions(this);
  },

  customSettingsButtons: computed(function () {
    const buttons = [];
    const reportButtons = this.reports.getReportButtons();
    $.extend(true, buttons, reportButtons);
    return buttons;
  }),

  organizationLimitPredicate: computed(function () {
    const actuality = new SimplePredicate('actuality', FilterOperator.Eq, true),
      orgType = new SimplePredicate('organizationType',
        FilterOperator.Eq, OrgTypeKeys.Municipality);

    return new ComplexPredicate(Condition.And, actuality, orgType);
  }),

  baseSignRole: computed(function () {
    return SignRoleEnum.commissionCreator;
  }),

  rolesToAdd: computed(function () {
    const result = [],
      i18n = this.i18n;
    result.push({
      key: SignRoleEnum.commissionCreator,
      name: i18n.t('forms.sign-role.add-commissionCreator'),
    });
    return result;
  }),

  getCellComponent(attr, bindingPath, model) {
    let cellComponent = this._super(...arguments);
    if (model && model.modelName === 'awp-readiness-passport-sign') {
      cellComponent = this.actSignProvider.getCellComponent(attr, bindingPath, model, cellComponent);
    }
    else {
      cellComponent.componentName = 'flexberry-field';
      cellComponent.componentProperties = {
        readonly: true
      };
    }
    return cellComponent;
  },

  isNotNewReadonly: computed('model.isNew', 'readonly', function () {
    return !this.get('model.isNew') || this.get('readonly');
  }),

});
