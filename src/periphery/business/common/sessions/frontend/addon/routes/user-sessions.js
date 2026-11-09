import ListFormRoute from '@timepath/tmobis-core/routes/base/list-form';
import UponListFilterRoute from '@timepath/tmobis-core/mixins/upon-list-filter-route';
import config from 'ember-get-config';
import { set } from '@ember/object';
import { inject as service } from '@ember/service';

export default ListFormRoute.extend(UponListFilterRoute, {
  sessionProvider: service('providers/user-sessions'),

  modelProjection: 'SessionL',
  modelName: 'user-session',
  templateName: 'user-sessions',

  filterSessionDateBegin: undefined,
  filterSessionDateEnd: undefined,
  filterShowArchived: undefined,

  /**
   * Настройки фильтрации списка.
   */
  // eslint-disable-next-line ember/avoid-leaking-state-in-ember-objects
  listFilterOptions: {
    filterModelName: 'user-sessions-filter'
  },

  // eslint-disable-next-line ember/avoid-leaking-state-in-ember-objects
  developerUserSettings: { 'sessionList': {
    DEFAULT: {
        "perPage": config.APP.resultsPerPage, 
        "columnWidths": [
          { "propName": "OlvRowMenu", "fixed": true },
          { "propName": "OlvRowToolbar", "fixed": true },
          { "propName": "actuality", "width": 30 },
        ],
        "sorting": [{ "propName": "startedAt", "direction": "desc" }]
      }
    }
  },

  model() {
    return this.getFilterModel(...arguments);
  },
  
  setupController(controller) {
    this._super(...arguments);
    this.setupFilterOptionsToController(controller);

    set(controller, 'recordsModelName', this.modelName);
    set(controller, 'filterSessionDateBegin', this.filterSessionDateBegin);
    set(controller, 'filterSessionDateEnd', this.filterSessionDateEnd);
    set(controller, 'filterShowArchived', this.filterShowArchived);

    if (controller.state === 'loading') {
      controller.set('state', '');
    }
  },

  /**
   * Загружает специфичную для формы информацию для фильтрации.
   * @param {Object} params Параметры запроса.
   * @param {Object} filterModel Модель для фильтрации.
   * @returns {Object} Специфичные для данной модели свойства с их значениями.
   */
  customLoadFilterValues(params, filterModel) {
    const sessionDateBegin = filterModel.get('sessionDateBegin') || null,
          sessionDateEnd = filterModel.get('sessionDateEnd') || null,
          showArchived = filterModel.get('showArchived') || false;

      this.set('filterSessionDateBegin', sessionDateBegin);
      this.set('filterSessionDateEnd', sessionDateEnd);
      this.set('filterShowArchived', showArchived);

      return Promise.resolve({
        sessionDateBegin, sessionDateEnd, showArchived
      });
  },

  /**
   * Возвращает предикат для фильтрации загружаемого списка.
   * @param {Object} options Параметры текущего роута.
   * @returns {Object} Предикат фильтрации (наследник BasePredicate).
   */
  objectListViewLimitPredicate() {
    let basePredicate = this._super(...arguments),
        sessionDateBegin =  this.filterSessionDateBegin,
        sessionDateEnd = this.filterSessionDateEnd,
        sessionArchived = this.filterShowArchived || false;

    return this.sessionProvider.getAllSessionsLimit(
      basePredicate, sessionDateBegin, sessionDateEnd, sessionArchived);
  },
});
