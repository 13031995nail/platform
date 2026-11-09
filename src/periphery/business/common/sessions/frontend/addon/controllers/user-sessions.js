import ListFormController
  from '@timepath/tmobis-core/controllers/base/list-form-common';
import UponListFilterController
  from '@timepath/tmobis-core/mixins/upon-list-filter-controller';
import { inject as service } from '@ember/service';
import { computed } from '@ember/object';

export default ListFormController.extend(UponListFilterController, {
  userSessions: service('providers/user-sessions'),

  /**
    @property title
    @type {String}
    @description Наименование таблицы.
  */
  title: 'Пользовательские сессии',
  
  /**
    @property ulfRowViewSettings
    @type {Object}
    @description Настройки для upon-list-filter, который выносится над таблицей.
  */
  ulfRowViewSettings: computed(function () {
    return {
      template: 'user-sessions/upon-list-filter-rowview',
      model: this.get('model'),
      setupFilterAction: this.get('actions.setUpFilters'),
      updateLookupValue: this.get('actions.updateLookupValue')
    }
  }),

  init() {
    this._super(...arguments);
    this.set('sessionAddItems', [{
      icon: 'delete icon',
      title: 'Завершить',
      actionName: 'stopSessionAction',
    }]);
  },

  getCellComponent: function (attr, bindingPath) {
    let cellComponent = this._super(...arguments);

    if (bindingPath === 'startedAt') {
      cellComponent.componentName = 'object-list-view-cell';
      cellComponent.componentProperties = {
        dateFormat: 'DD.MM.YYYY HH:mm'
      };
    }

    return cellComponent;
  },

  actions: {
    async stopSession(session) {
      const sessionId = session.id;
      await this.userSessions.stopOtherSession(sessionId);
      this.send('refreshList');
    },
  }
});
