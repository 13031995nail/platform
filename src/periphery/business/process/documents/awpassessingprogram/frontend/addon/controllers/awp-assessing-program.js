import EditFormController from '@timepath/tmobis-business-process-acts-base-act/controllers/documents-edit-base';

import { computed } from '@ember/object';
import { inject as service } from '@ember/service';
import $ from 'jquery';

import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { ComplexPredicate, SimplePredicate } from 'ember-flexberry-data/query/predicate';

import OrgTypeKeys from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';
import SignRoleEnum from '@timepath/tmobis-business-process-acts-actsign/enums/sign-role';

export default EditFormController.extend({
  actStatusProvider: service(),
  actSignProvider: service('act-sign-provider'),
  reports: service(),
  httpService: service('http-request'),

  parentRoute: 'awp-assessing-programs',

  init() {
    this._super(...arguments);
    this.reports.bindActions(this);
  },

  customSettingsButtons: computed('model.actStatus', 'canSign', function () {
    const result = [],
      reportButtons = this.reports.getReportButtons(),
      statusId = this.get('model.actStatus.id'),
      isSigned = statusId === ActStatusKeys.signed,
      canSign = this.get('canSign'),
      i18n = this.i18n;

    $.extend(true, result, reportButtons);

    if (isSigned && canSign) {
      result.push({
        Icon: 'check',
        Caption: i18n.t('forms.admission-committee.approve-caption'),
        Action: 'approve'
      });
    }
    return result;
  }),

  organizationLimitPredicate: computed(function () {
    const actuality = new SimplePredicate('actuality', FilterOperator.Eq, true),
      orgType = new SimplePredicate('organizationType',
        FilterOperator.Eq, OrgTypeKeys.Municipality);

    return new ComplexPredicate(Condition.And, actuality, orgType);
  }),
  baseSignRole: computed(function () {
    return SignRoleEnum.commissionChair;
  }),
  rolesToAdd: computed(function () {
    const result = [],
      i18n = this.i18n;
    result.push({
      key: SignRoleEnum.commissionChair,
      name: i18n.t('forms.sign-role.add-commissionChair'),
    });
    return result;
  }),

  getCellComponent(attr, bindingPath, model) {
    let cellComponent = this._super(...arguments);
    let updateLookupValue = this.get('actions.updateLookupValue').bind(this);
    if (model && model.modelName === 'awp-assessing-program-sign') {
      cellComponent = this.actSignProvider.getCellComponent(attr, bindingPath, model, cellComponent, updateLookupValue);
    }
    else if (bindingPath === 'objectFunction') {
      let updateLookupValue = this.get('actions.updateLookupValue').bind(this);
      cellComponent.componentProperties = {
        choose: 'showLookupDialog',
        chooseText: '...',
        remove: 'removeLookupValue',
        displayAttributeName: 'name',
        readonly: true,
        relationName: 'objectFunction',
        projection: this.get('modelProjection.attributes.schedules'),
        updateLookupValue: updateLookupValue,
        lookupLimitPredicate: this.actualityCustomLimitPredicate
      };
    } else if (bindingPath == 'objectsCount') {
      cellComponent.componentProperties.readonly = true;
    }
    return cellComponent;
  },

  isNotNewReadonly: computed('model.isNew', 'readonly', function () {
    return !this.get('model.isNew') || this.get('readonly');
  }),

  async approve() {
    const model = this.get('model'),
      modals = this.get('modals');

    const errors = await this.validateModel();
    if (errors.length > 0) {
      modals.showModal({
        title: 'Необходимо заполнить обязательные поля',
        message: errors
      });
      return;
    }

    modals.showLoading({ title: 'Утверждение...' });
    const actStatus = await this.actStatusProvider
      .getActStatus(ActStatusKeys.approved);
    model.set('actStatus', actStatus);
    model.set('approvedDate', new Date());
    this.set('readonly', true);
    try {
      await model.save();
      modals.hideLoading(true);
    }
    catch (ex) {
      modals.hideLoading(true);
      if (ex) {
        modals.showModal({
          message: ex.msg
        });
      }
    }
  },

  validateModel() {
    let errors = [];
    const model = this.get('model');

    const hasResolutionNumber = model.get('resolutionNumber');
    if (!hasResolutionNumber) {
      errors.push('Необходимо указать номер постановления о создании комиссии');
    }
    const hasResolutionDate = model.get('resolutionDate');
    if (!hasResolutionDate) {
      errors.push('Необходимо указать дату постановления о создании комиссии');
    }

    const schedules = model.get('schedules');
    const validSchedules = schedules.every(schedule => {
      return schedule.get('startDate') && schedule.get('finishDate');
    });
    if (!validSchedules) {
      errors.push('Необходимо указать даты проверки в графике проведения оценки');
    }

    return errors;
  },

  actions: {
    approve() {
      this.approve();
    },
  }
});
