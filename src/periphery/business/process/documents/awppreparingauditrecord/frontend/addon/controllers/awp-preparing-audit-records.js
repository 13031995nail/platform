import ListFormController from '@timepath/tmobis-core/controllers/base/list-form-common';
import UponListFilterController from '@timepath/tmobis-core/mixins/upon-list-filter-controller';
import { computed } from '@ember/object';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { ComplexPredicate, SimplePredicate } from 'ember-flexberry-data/query/predicate';
import { isBlank, isNone, isEmpty } from '@ember/utils';
import config from 'ember-get-config';
import $ from 'jquery';
import { inject as service } from '@ember/service';
// eslint-disable-next-line ember/no-observers
import { observer } from '@ember/object';
import { once } from '@ember/runloop';

import OrganizationTypeEnum from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';

export default ListFormController.extend(UponListFilterController, {
  editFormRoute: 'awp-preparing-audit-record',
  recordsModelName: undefined,
  committee: undefined,
  modals: service(),
  rolesManager: service(),
  session: service(),
  committeeProvider: service('admission-committee-provider'),

  /**
 * Флаг видимости диалога ручного добавления объекта теплопотребления.
 */
  addObjectsDialogIsVisible: false,

  orgMOCustomLimitPredicate: computed(function () {
    return new ComplexPredicate(Condition.And, ...[
      new SimplePredicate('organizationType', FilterOperator.Eq, OrganizationTypeEnum.Municipality),
      new SimplePredicate('actuality', FilterOperator.Eq, true)
    ]);
  }),

  /**
   * Ограничивает лукап «Объекты» только списком допустимых id,
   * и пока идёт загрузка — подсовывает «невыполнимый» предикат.
   */
  objectCustomLimitPredicate: computed(
    'model.availableObjectIds.[]',
    function () {
      // Допустимые города (строковые GUID'ы).
      const ids = (this.get('model.availableObjectIds') || []).map(String);

      if (!ids || ids.length === 0) {
        return new SimplePredicate('id', FilterOperator.Eq, '00000000-0000-0000-0000-000000000000');;
      }

      // Собираем OR из Eq.
      const orParts = ids.map(id => new SimplePredicate('id', FilterOperator.Eq, id));
      const orPredicate = orParts.length === 1
        ? orParts[0]
        : new ComplexPredicate(Condition.Or, ...orParts);

      return orPredicate;
    }
  ),

  // eslint-disable-next-line ember/no-observers
  awpObserver: observer('model.awp', function () {
    once(this, 'findCommittee');
  }),
  async findCommittee() {
    const awp = this.get('model.awp');

    if (!awp) {
      this.set('committee', undefined);
      return;
    }

    const committee = await this.committeeProvider.getApprovedCommitteeByLead(this.session.data.authenticated.employee, awp.get('id'));
    this.set('committee', committee);
  },

  /**
   * Информация о пунктах меню "Настройки", открывающегося по кнопке тулбара.
   */
  menuAdditionalItems: computed('session.data.authenticated.roles', 'committee', function () {
    let self = this,
      result = this._super(...arguments);
    
    if (!isNone(this.committee)) {
      result[0].items.push({
        iconClass: 'arrow-down-circle',
        title: 'Сформировать журнал',
        onItemClickAction: () => {
          self.send('createAwpPreparingAuditRecords');
        }
      }, {
        iconClass: 'plus-circle',
        title: 'Добавить объект теплопотребления',
        onItemClickAction: () => {
          self.send('addObjectManually');
        }
      });
    } 
    return result;
  }),

  actions: {
    createAwpPreparingAuditRecords() {
      let self = this,
        awp = this.get('model.awp'),
        modals = this.get('modals');

      if (isEmpty(awp)) {
        modals.showAlert({ message: 'Выберите осенне-зимний период' });
        return;
      }

      modals.showLoading({ message: 'Формирование журнала...' });
      let token = self.get('session.data.authenticated.token') || self.get('session.data.authenticated.access_token');
      $.ajax({
        url: config.APP.backendUrls.root + "/api/AwpPreparingAuditRecord/UpdateRecords",
        type: "GET",
        data: {
          awpId: awp.get('id'),
          employeeId: this.session.data.authenticated.employee
        },
        headers: { 'Authorization': 'Bearer ' + token },
        success(data) {
          modals.hideLoading(true);
          self.send('refreshList');
        },
        error(xhr, status, e) {
          modals.hideLoading(true);
          modals.showAlert({ message: xhr.responseJSON.detail });
        }
      });
    },

    createRecordsForSpecifiedObjects() {
      this.set('addObjectsDialogIsVisible', false);

      let consObj = this.get('model.workObjectToCreateRecord'),
        self = this,
        awp = this.get('model.awp'),
        modals = this.get('modals');

      let selectedWorkObject = consObj.get('id');

      modals.showLoading({ message: 'Добавление объекта в журнал...' });
      let token = this.get('session.data.authenticated.token') || this.get('session.data.authenticated.access_token');
      $.ajax({
        url: config.APP.backendUrls.root + "/api/AwpPreparingAuditRecord/UpdateRecordsForObjects",
        type: "GET",
        data: {
          awpId: awp.get('id'),
          employeeId: this.session.data.authenticated.employee,
          workObjectId: selectedWorkObject
        },
        headers: { 'Authorization': 'Bearer ' + token },
        success(data) {
          if (data === 0) {
            modals.showAlert({ message: 'Запись журнала уже существует.' });
          }
          modals.hideLoading(true);
          self.get('model').set('workObjectToCreateRecord', null);
          self.send('refreshList');
        },
        error(xhr, status, e) {
          modals.hideLoading(true);
          modals.showAlert({ message: xhr.responseJSON.detail });
        }
      });
    },

    addObjectManually() {
      if (!this.get('model.awp')) {
        let modals = this.get('modals');
        modals.showAlert({ message: 'Выберите осенне-зимний период' });
        return;
      }
      let self = this,
        awp = this.get('model.awp'),
        modals = this.get('modals');
      modals.showLoading({ message: 'Загрузка доступных объектов...' });
      const ids = this.get('model.availableObjectIds');
      ids.clear();
      let token = this.get('session.data.authenticated.token') || this.get('session.data.authenticated.access_token');
      $.ajax({
        url: config.APP.backendUrls.root + "/api/AwpPreparingAuditRecord/AvailableObjects",
        type: 'GET',
        dataType: 'json',
        data: { awpId: awp.get('id'), employeeId: this.session.data.authenticated.employee  },
        headers: { 'Authorization': 'Bearer ' + token },
        success: (data) => {
          const newIds = (Array.isArray(data) ? data : []).map(String);
          ids.addObjects(newIds);
          modals.hideLoading(true);
          self.set('addObjectsDialogIsVisible', true);
        },
        error: (xhr, status, err) => {
          modals.hideLoading(true);
          modals.showAlert({ message: xhr.responseJSON.detail });
        }
      });
    },

    cancelCreateRecordsForSpecifiedObjects() {
      this.set('addObjectsDialogIsVisible', false);
    },

    removeLookupValue(updateData) {
      this._super(...arguments);
      const isAwpChanged = updateData.relationName === 'awp';
      const isOrganizationChanged = updateData.relationName === 'organization';
      const isObjectFunctionChanged = updateData.relationName === 'objectFunction';
      const isOrganizationTypeChanged = updateData.relationName === 'organizationType';
      if (isAwpChanged || isOrganizationChanged || isObjectFunctionChanged || isOrganizationTypeChanged) {
        this.send('refresh');
      }
    },

    updateLookupValue(updateData) {
      this._super(...arguments);
      const isAwpChanged = updateData.relationName === 'awp';
      const isOrganizationChanged = updateData.relationName === 'organization';
      const isObjectFunctionChanged = updateData.relationName === 'objectFunction';
      const isOrganizationTypeChanged = updateData.relationName === 'organizationType';
      if (isAwpChanged || isOrganizationChanged || isObjectFunctionChanged || isOrganizationTypeChanged) {
        this.send('refresh');
      }
    }
  }
});
