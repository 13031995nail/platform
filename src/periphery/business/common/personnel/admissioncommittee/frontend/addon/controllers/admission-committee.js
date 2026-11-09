import DictionariesEditFormController from '@timepath/tmobis-core/controllers/base/edit-form-common';

import { computed, set } from '@ember/object';
import { isEmpty } from '@ember/utils';
import { inject as service } from '@ember/service';

import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { ComplexPredicate, SimplePredicate } from 'ember-flexberry-data/query/predicate';
import Builder from 'ember-flexberry-data/query/builder';

import OrganizationTypeEnum from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';
import AdmissionCommitteeStatusEnum from '../enums/admission-committee-status';

export default DictionariesEditFormController.extend({
  parentRoute: 'admission-committees',
  appState: service(),
  changeField: false,

  buildEmployeeItems(employees, excludedId) {
    let dict = {};
    employees.forEach(employee => {
      if (!isEmpty(employee.get('employee.fio')) && employee.id !== excludedId) {
        dict[employee.id] = employee.employee.fio;
      }
    });

    return dict;
  },

  resolveSelectedKey(value, fallbackValue) {
    if (value === undefined) {
      return fallbackValue;
    }

    return value;
  },

  customSettingsButtons: computed('model.status', function () {
    const result = [],
      status = this.get('model.status'),
      i18n = this.i18n;
    if (!this.get('model.isNew')) {
      if (status !== AdmissionCommitteeStatusEnum.Approved) {
        result.push({
          Icon: 'check',
          Caption: i18n.t('forms.admission-committee.approve-caption'),
          Action: 'approve'
        });
      }

      if (status !== AdmissionCommitteeStatusEnum.Editing) {
        result.push({
          Icon: 'reply',
          Caption: i18n.t('forms.admission-committee.unapprove-caption'),
          Action: 'unapprove'
        });
      }
    }
    return result;
  }),

  organizationCustomLimitPredicate: computed(function () {
    return new ComplexPredicate(Condition.And, ...[
      new SimplePredicate('organizationType', FilterOperator.Eq, OrganizationTypeEnum.Municipality),
      new SimplePredicate('actuality', FilterOperator.Eq, true)
    ]);
  }),

  createEmployeePredicate() {
    let admissions = this.get('model.admissionCommitteeEmployee'),
      employees = admissions.filter(x => !isEmpty(x.get('employee')) && !x.get('isDeleted')),
      organization = this.get('model.organization'),
      predicates = [new SimplePredicate('employee.actuality', FilterOperator.Eq, true)];
    if (employees.length > 0) {
      predicates = predicates.concat(employees.map(x => new SimplePredicate('employee.id', FilterOperator.Neq, x.get('employee.id'))));
    }
    if (!isEmpty(organization)) {
      predicates.push(new SimplePredicate('organization.hierarchy.id', FilterOperator.Eq, organization.id))
    }
    return predicates.length > 1 ? new ComplexPredicate(Condition.And, ...predicates) : predicates[0];
  },

  createEmployeeDict(employeeId) {
    let dict = {},
      employees = this.get('model.admissionCommitteeEmployee');
    employees.forEach(x => {
      set(x, 'employee', x.get('placeOfEmployment.employee'));
    });
    employees.forEach(employee => {
      if (!isEmpty(employee.get('employee.fio')) && !employee.get('isDeleted') && (employee.id !== employeeId)) {
        dict[employee.id] = employee.employee.fio;
      }
    });
    return dict;
  },

  leaderItems: computed(
    'changeField',
    'model.admissionCommitteeEmployee',
    'model.admissionCommitteeEmployee.length',
    'model.admissionCommitteeEmployee.@each.placeOfEmployment',
    'model.admissionCommitteeEmployee.@each.isDeleted', function () {
      let leaderId = this.resolveSelectedKey(this.get('leaderKey'), this.get('model.leader.id')),
        deputyLeaderId = this.resolveSelectedKey(this.get('deputyLeaderKey'), this.get('model.deputyLeader.id')),
        dict = this.createEmployeeDict(deputyLeaderId);
      if (!isEmpty(leaderId) && isEmpty(dict[leaderId])) {
        this.set('leaderKey', null);
        this.set('model.leader', null);
      }
      this.set('employeeCustomLimitPredicate', this.createEmployeePredicate());
      return dict;
    }),

  deputyLeaderItems: computed(
    'changeField',
    'model.admissionCommitteeEmployee',
    'model.admissionCommitteeEmployee.length',
    'model.admissionCommitteeEmployee.@each.placeOfEmployment',
    'model.admissionCommitteeEmployee.@each.isDeleted', function () {
      let deputyLeaderId = this.resolveSelectedKey(this.get('deputyLeaderKey'), this.get('model.deputyLeader.id')),
        leaderId = this.resolveSelectedKey(this.get('leaderKey'), this.get('model.leader.id')),
        dict = this.createEmployeeDict(leaderId);
      if (!isEmpty(deputyLeaderId) && isEmpty(dict[deputyLeaderId])) {
        this.set('deputyLeaderKey', null);
        this.set('model.deputyLeader', null);
      }
      this.set('employeeCustomLimitPredicate', this.createEmployeePredicate());
      return dict;
    }
  ),

  getCellComponent(attr, bindingPath) {
    let cellComponent = this._super(...arguments);
    let updateLookupValue = this.get('actions.updateLookupValue').bind(this);
    if (attr.kind === 'belongsTo') {
      if (bindingPath === 'placeOfEmployment') {
        cellComponent.componentProperties = {
          choose: 'showLookupDialog',
          chooseText: '...',
          remove: 'removeLookupValue',
          displayAttributeName: 'employee.fio',
          required: true,
          relationName: 'placeOfEmployment',
          projection: 'EmployeeAdmissionCommitteeL',
          updateLookupValue: updateLookupValue,
          lookupLimitPredicate: this.employeeCustomLimitPredicate
        };
      }
    }
    if (bindingPath === 'placesOfWork') {
      cellComponent.componentName = 'flexberry-field';
      cellComponent.componentProperties = {
        disabled: true
      };
    }
    return cellComponent;
  },

  changeStatus(status, date, message) {
    let self = this,
      model = self.get('model'),
      modals = this.get('modals');
    if (model.get('admissionCommitteeEmployee.length') < 5) {
      return modals.showModal({
        message: 'В комиссии должно быть не менее 5 человек'
      });
    } else {
      this.appState.loading();
      model.set('status', status);
      model.set('approvedDate', date);
      return model.save().then(() => {
        self.appState.reset();
      }).catch((errorData) => {
        self.appState.reset();
        if (errorData) {
          modals.showModal({
            message: errorData.msg
          });
        }
      });
    }
  },

  async _checkExistACEmployee() {
    let builder = new Builder(this.store)
      .from('admission-committee')
      .selectByProjection('AdmissionCommitteeE')
      .where(new ComplexPredicate(Condition.And,
        new SimplePredicate('awp.id', FilterOperator.Eq, this.get('model.awp.id')),
        new SimplePredicate('organization.id', FilterOperator.Eq, this.get('model.organization.id')))),
      records = await this.store.query('admission-committee', builder.build());
    return records;
  },

  async customPresave() {
    let records = await this._checkExistACEmployee(),
      isNew = this.get('model.isNew'),
      promises = [],
      isExist = records.length > 0;

    if (isNew && isExist) {
      let record = records.firstObject;
      await this.modals.showModal({
        message: `Комиссия выбранного МО на ОЗП ${record.awp.name} была создана. Перейти?`,
        onApprove: () => { this.transitionToRoute('admission-committee', record.id); },
        onDeny: () => {}
      });

      return this.createSaveResult(false, null, true);
    }

    let employees = this.get('model.admissionCommitteeEmployee');
    let leaderKey = this.resolveSelectedKey(this.get('leaderKey'), this.get('model.leader.id'));
    if (isEmpty(leaderKey)) {
      return this.createSaveResult(false, 'Председатель не указан');
    }
    let leader = employees.find(val => val.id === leaderKey);
    if (isEmpty(leader)) {
      return this.createSaveResult(false, 'Председатель не указан');
    }

    let deputyLeaderKey = this.resolveSelectedKey(
      this.get('deputyLeaderKey'),
      this.get('model.deputyLeader.id')
    );
    let deputyLeader = null;

    if (!isEmpty(deputyLeaderKey)) {
      deputyLeader = employees.find(val => val.id === deputyLeaderKey) || null;
    }

    if (!isEmpty(deputyLeader) && leader.id === deputyLeader.id) {
      return this.createSaveResult(false, 'Председатель комиссии не может быть заместителем председателя');
    }

    this.set('model.leader', leader);
    this.set('model.deputyLeader', deputyLeader);

    employees.forEach(x => { if (!isNew) { promises.push(x.save()); } });
    this.appState.loading();
    await Promise.all(promises);
    this.appState.reset();
    return this.createSaveResult(true);
  },

  actions: {
    approve() {
      this.changeStatus(AdmissionCommitteeStatusEnum.Approved, new Date(), 'Утверждение...');
    },
    unapprove() {
      this.changeStatus(AdmissionCommitteeStatusEnum.Editing, null, 'Отмена утверждения...');
    },
    onChangeLeaderDropdown(value) {
      if (!isEmpty(value) || value !== '') {
        this.set('leaderKey', value);
        this.set('changeField', !this.changeField);
      }
    },
    onChangeDeputyLeaderDropdown(value) {
      if (!isEmpty(value) || value !== '') {
        this.set('deputyLeaderKey', value);
        this.set('changeField', !this.changeField);
      }
    },
    showLookupDialog(data) {
      if (!Ember.isEmpty(data) && !Ember.isEmpty(data.componentName)
        && data.componentName.match(/\(AdmissionCommittee-employees_flexberry-lookup_placeOfEmployment*/)) {
        data.predicate = this.employeeCustomLimitPredicate;
        data.groupBy = 'employee.id';
      }
      this._super(...arguments);
    }
  }
});
