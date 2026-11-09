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

  modelProjection: 'AwpAssessingProgramL',
  modelName: 'awp-assessing-program',

  masterTitle: t('forms.awp-assessing-programs.caption'),

  limitfunction: null,
  filterAwp: undefined,

  listFilterOptions: {
    filterByAwp: true,
    filterModelName: 'awp-assessing-program-filter',
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
        awp: awp
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
          "sorting": [{ "propName": "regDate", "direction": "desc" }],
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
