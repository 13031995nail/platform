/* eslint-disable ember/no-observers */
import EditFormController from '@timepath/tmobis-business-process-acts-base-act/controllers/documents-edit-base';
import { computed, observer } from '@ember/object';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate }
  from 'ember-flexberry-data/query/predicate';
import Condition from 'ember-flexberry-data/query/condition'
import { isNone } from '@ember/utils';
import { inject as service } from '@ember/service';
import { once } from '@ember/runloop';
import SignRoleEnum from '@timepath/tmobis-business-process-acts-actsign/enums/sign-role';
import $ from 'jquery';

export default EditFormController.extend({
  actStatusProvider: service(),
  actSignProvider: service('act-sign-provider'),
  placeOfEmploymentProvider: service('place-of-employment-provider'),
  reports: service(),
  httpService: service('http-request'),

  parentRoute: 'rediness-acts',

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

  isNotNewReadonly: computed('model.isNew', 'readonly', function () {
    return !this.get('model.isNew') || this.get('readonly');
  }),
  baseSignRole: computed(function () {
    return SignRoleEnum.commissionChair;
  }),
  rolesToAdd: computed(function () {
    const result = [],
      i18n = this.i18n;
    result.push({
      key: SignRoleEnum.organizationRepresentative,
      name: i18n.t('forms.sign-role.add-organizationRepresentative'),
    });
    result.push({
      key: SignRoleEnum.commissionChair,
      name: i18n.t('forms.sign-role.add-commissionChair'),
    });
    result.push({
      key: SignRoleEnum.commissionDeputyChair,
      name: i18n.t('forms.sign-role.add-commissionDeputyChair'),
    });
    result.push({
      key: SignRoleEnum.commissionMember,
      name: i18n.t('forms.sign-role.add-commissionMember'),
    });
    return result;
  }),
  getCellComponent(attr, bindingPath, model) {
    let cellComponent = this._super(...arguments);
    if (model && model.modelName === 'readiness-act-sign') {
      cellComponent = this.actSignProvider.getCellComponent(attr, bindingPath, model, cellComponent);
    }
    else if (model.modelName === 'rediness-act-object' && bindingPath !== 'objectReadiness')
      cellComponent.componentName = 'flexberry-field';
    cellComponent.componentProperties = {
      readonly: true
    };
    return cellComponent;
  },
  assigneeCustomLimitPredicate: undefined,
  organizationObserver: observer('model.organization', function () {
    once(this, 'onOrganizationChanged');
  }),
  async onOrganizationChanged() {
    let self = this,
      organization = this.get('model.organization');
    if (!isNone(organization)) {
      let places = await this.placeOfEmploymentProvider.findActualPlaceForEmployee(organization);
      let prs = [];
      places.forEach(function (element) {
        let employee = element.get('employee');
        if (!isNone(employee)) {
          prs.push(new SimplePredicate('id', FilterOperator.Eq, employee.get('id')));
        }
      }, this);
      if (prs.length > 0) {
        self.set('assigneeCustomLimitPredicate',
          new ComplexPredicate(Condition.And, (prs.length === 1 ? prs[0] : new ComplexPredicate(Condition.Or, ...prs)),
            new SimplePredicate('actuality', FilterOperator.Eq, true)));
      } else {
        self.set('assigneeCustomLimitPredicate', new SimplePredicate('id', FilterOperator.Eq, 'null'));
      }
    } else {
      self.set('assigneeCustomLimitPredicate', new SimplePredicate('id', FilterOperator.Eq, 'null'));
    }
  },
  actions: {
  }
});
