import ListFormRoute from '@timepath/tmobis-core/routes/base/list-form';
import config from 'ember-get-config';
import { set } from '@ember/object';
import UponListFilterRoute from '@timepath/tmobis-core/mixins/upon-list-filter-route';
import { computed } from '@ember/object';
import { inject as service } from '@ember/service';
import { A } from '@ember/array';
import { isEmpty } from '@ember/utils';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { translationMacro as t } from 'ember-i18n';

export default ListFormRoute.extend(UponListFilterRoute, {
  awpProvider: service(),
  //employeeProvider: service(),

  modelProjection: 'AwpPreparingAuditRecordL',
  modelName: 'awp-preparing-audit-record',

  masterTitle: t('forms.awp-preparing-audit-records.caption'),

  limitfunction: null,
  filterAwp: undefined,
  filterOrganization: undefined,
  filterObjectFunction: undefined,
  filterOrganizationType: undefined,

  listFilterOptions: {
    filterByAwp: true,
    filterModelName: 'awp-preparing-audit-record-filter',
    awpPropPath: 'awp'
  },

  model(params, transition) {
    return this.getFilterModel(params, transition).then(model => model);
  },

  setupController(controller) {
    this._super(...arguments);
    this.setupFilterOptionsToController(controller);

    set(controller, 'recordsModelName', this.get('modelName'));
    set(controller, 'filterAwp', this.get('filterAwp'));
    set(controller, 'filterOrganization', this.get('filterOrganization'));
    set(controller, 'filterObjectFunction', this.get('filterObjectFunction'));
    set(controller, 'filterOrganizationType', this.get('filterOrganizationType'));
    //const currentEmployee = await this.employeeProvider.getCurrentEmployee();
    //set(controller, 'employee', currentEmployee);
  },

  resetController(controller, isExiting) {
    this._super(...arguments);
    if (isExiting) {
      controller.set('awpId', undefined);
      controller.set('organizationId', undefined);
      controller.set('objectFunctionId', undefined);
      controller.set('organizationTypeId', undefined);
    }
  },

  /**
   * Загружает из удалённого хранилища значения для фильтрации.
   * Используется только 1 раз, при первичном открытии формы.
   */
  _loadValuesToFilter() {
    const self = this;

    return Promise.all([
      this.awpProvider.getCurrentAwp(),
      this.setPermissions()
    ]).then(values => {
      const awp = values[0];
      self.set('filterAwp', awp);

      return this.get('store').createRecord(
        self.get('listFilterOptions.filterModelName'), {
          awp: awp,
      });
    });
  },

  /**
   * Проставляет значения полей роута из модели, загруженной из локального хранилища.
   * @param {DS.Record} filterModel Модель для фильтрации.
   */
  _peekValuesToFilter(filterModel) {
    const self = this;
    return this.setPermissions().then(() => new Promise(resolve => {
      self.set('filterAwp', filterModel.get('awp'));
      self.set('filterOrganization', filterModel.get('organization'));
      self.set('filterObjectFunction', filterModel.get('objectFunction'));
      self.set('filterOrganizationType', filterModel.get('organizationType'));
      return resolve(filterModel);
    }));
  },

  objectListViewLimitPredicate() {
    let basePredicate = this._super(...arguments),
      awpId = this.get('filterAwp.id'),
      organizationId = this.get('filterOrganization.id'),
      objectFunctionId = this.get('filterObjectFunction.id'),
      organizationTypeId = this.get('filterOrganizationType.id'),
      conditions = [];

    if (!isEmpty(awpId) || !isEmpty(organizationId) || !isEmpty(objectFunctionId) || !isEmpty(organizationTypeId)) {
      if (basePredicate) {
        conditions.push(basePredicate);
      }

      if (this.get('listFilterOptions.filterByAwp') && !isEmpty(awpId)) {
        conditions.push(new SimplePredicate(this.get('listFilterOptions.awpPropPath'), FilterOperator.Eq, awpId));
      }

      if (!isEmpty(organizationId)) {
        conditions.push(new SimplePredicate('workObject.organization.hierarchy', FilterOperator.Eq, organizationId));
      }

      if (!isEmpty(objectFunctionId)) {
        conditions.push(new SimplePredicate('workObject.objectFunction', FilterOperator.Eq, objectFunctionId));
      }

      if (!isEmpty(organizationTypeId)) {
        conditions.push(new SimplePredicate('workObject.organization.organizationType', FilterOperator.Eq, organizationTypeId));
      }

      return conditions.length === 0
        ? basePredicate
        : (conditions.length === 1
          ? conditions[0]
          : new ComplexPredicate(Condition.And, ...conditions));
    }

    return basePredicate;

  },

  developerUserSettings: computed(function () {
    return {
      'objectlistviewComponentName': {
        'DEFAULT': {
          'perPage': config.APP.components.flexberryObjectlistview.defaultPerPage,
          'columnWidths': [
            { 'propName': 'OlvRowMenu', 'fixed': true },
            { 'propName': 'OlvRowToolbar', 'fixed': true }
          ]
        }
      }
    }
  }),

  actions: {
    willTransition(transition) {
      this._super(...arguments);

      if (transition.targetName !== this.routeName) {
        this.controller.set('changeRoute', true);
        this.controller.set('selectedRecords', A());
        this.controller.set('selectedRecordsCache', A());
      }
    }
  }
});
