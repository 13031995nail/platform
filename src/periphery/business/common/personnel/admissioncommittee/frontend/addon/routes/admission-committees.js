import ListFormRoute from '@timepath/tmobis-core/routes/base/list-form';
import config from 'ember-get-config';
import { computed } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';
import UponListFilterRoute from '@timepath/tmobis-core/mixins/upon-list-filter-route';
import { inject as service } from '@ember/service';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { A } from '@ember/array';
import { isEmpty } from '@ember/utils';
import { set } from '@ember/object';

export default ListFormRoute.extend(UponListFilterRoute, {
  awpProvider: service('awp-provider'),

  modelProjection: 'AdmissionCommitteeL',
  modelName: 'admission-committee',

  masterTitle: t('forms.admission-committees.caption'),

  filterAwp: undefined,

  listFilterOptions: {
    filterByAwp: true,
    filterModelName: 'admission-committee-filter',
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
  },

  resetController(controller, isExiting) {
    this._super(...arguments);
    if (isExiting) {
      controller.set('awpId', undefined);
    }
  },

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
        awp: awp
      });
    });
  },

  _peekValuesToFilter(filterModel) {
    const self = this;
    return this.setPermissions().then(() => new Promise(resolve => {
      self.set('filterAwp', filterModel.get('awp'));
      return resolve(filterModel);
    }));
  },

  objectListViewLimitPredicate() {
    let basePredicate = this._super(...arguments),
      awpId = this.get('filterAwp.id'),
      conditions = [];

    if (!isEmpty(awpId)) {
      if (basePredicate) {
        conditions.push(basePredicate);
      }

      if (this.get('listFilterOptions.filterByAwp') && !isEmpty(awpId)) {
        conditions.push(new SimplePredicate(this.get('listFilterOptions.awpPropPath'), FilterOperator.Eq, awpId));
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
          ],
          "colsOrder": [
            { "propName": "regDate" },
            { "propName": "status" },
            { "propName": "organization.name" },
            { "propName": "leader.employee.fio", "hideFilter": true }
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
