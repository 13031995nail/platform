import Ember from 'ember';
import FlexberryBaseComponent from 'ember-flexberry/components/flexberry-base-component';
import { inject as service } from '@ember/service';
import { isNone, isBlank } from '@ember/utils';
import { computed, set } from '@ember/object';
import { resolve } from 'rsvp';
import { once } from '@ember/runloop';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate }
  from 'ember-flexberry-data/query/predicate';
import Condition from 'ember-flexberry-data/query/condition'

import ActStatusEnum from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';
import OrganizationTypeEnum from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';
import SignRoleEnum from '@timepath/tmobis-business-process-acts-actsign/enums/sign-role';

export default FlexberryBaseComponent.extend({
  signatureProvider: service('employee-signature-provider'),
  actStatusProvider: service('act-status-provider'),
  signRoleProvider: service('sign-role-provider'),
  placeOfEmploymentProvider: service('place-of-employment-provider'),
  admissionCommitteeProvider: service('admission-committee-provider'),
  actSignProvider: service('act-sign-provider'),
  /**
 * Сервис текущей сессии.
 */
  session: service(),
  modals: service(),
  store: service(),

  chooseEmployeeAction: 'showLookupDialog',
  /**
   @property showLookupDialog
   @description Наименование пробрасываемого события открытия лукапа.
 */
  showLookupDialog: 'showLookupDialog',
  readonly: false,
  hideAllButtons: false,

  /**
  * Идентификатор блочного элемента компонента.
  */
  wrapperId: null,

  /**
   * Представление для встроенного object-list-view.
   */
  signingProjection: 'ActSignD',

  /**
  * Имя компонента для встроенного object-list-view.
  */
  componentName: null,

  /**
    * Массив экземпляров act-sign, соответствующий подписантам текущего акта,
    *  открытого на редактирование.
  */
  content: null,

  /**
  * Модель, хранящая текущее состояние модуля подписания.
  */
  signingInternalModel: null,

  /**
    * Индикатор доступности подписания стилусом.
    */
  stylusSigningIsAvailable: false,

  stilusSigningVisible: false,

  /**
   * Настройки компонента для отображения ячеек для object-list-view с одной колонкой
   *  (в мобильном варианте).
   */
  singleColumnCellComponent: {
    componentName: 'sign-single-column-cell',
    componentProperties: null
  },

  /**
   * Функция, вызывающаяся после формирования DOM компонента.
   *
   * @returns {undefined}
   */
  didInsertElement() {
    let self = this,
      dimmerSettings = {
        opacity: 0.7,
        closable: false,
        onHide() {
          self.set('signingInternalModel.state', 'normal');
        }
      };

    this.$('#' + this.wrapperId).dimmer(dimmerSettings);
  },

  /**
   * Инициализация компонента.
   *
   * @returns {undefined}
   */
  init() {
    this._super(...arguments);
    this.set('wrapperId', 'actSigning' + Ember.uuid());
    this.set('signingInternalModel', this.store.createRecord('act-signing-internal-model', {
      state: 'normal'
    }));
  },

  /**
  * Доступность кнопки отписать всех.
  */
  isClearSigningButtonDisabled: computed('targetObject.model.actStatus', 'hideAllButtons',
    'content.[]',
    'content.@each.{employee.id,signRole.id}', 'session.data.authenticated.employee', 'baseSignRole', function () {
    let status = this.get('targetObject.model.actStatus.id');
    if (this.hideAllButtons || status !== ActStatusEnum.signing) {
      return true;
    }

    const employeeId = this.get('session.data.authenticated.employee');
    const baseSignRole = this.get('baseSignRole');
    const signs = this.get('content');
    const records = signs && signs.toArray ? signs.toArray() : (signs || []);
    return !employeeId || !baseSignRole || !records.some(record =>
      record.get('employee.id') === employeeId && record.get('signRole.id') === baseSignRole);
  }),

  /**
   * Видимость кнопки готов к подписанию.
   */
  isSignInspectorButtonDisabled: computed(
    'targetObject.model.actStatus',
    'hideAllButtons',
    'content.[]',
    'content.@each.{employee.id,signRole.id}',
    'session.data.authenticated.employee',
    'baseSignRole',
    function () {
    let status = this.get('targetObject.model.actStatus.id');

    if (this.hideAllButtons || status !== ActStatusEnum.editing) {
      return true;
    }

    const employeeId = this.get('session.data.authenticated.employee');
    const baseSignRole = this.get('baseSignRole');
    const signs = this.get('content');
    const records = signs && signs.toArray ? signs.toArray() : (signs || []);
    return !employeeId || !records.some(record =>
      record.get('employee.id') === employeeId &&
      (isNone(baseSignRole) || record.get('signRole.id') === baseSignRole));
  }),

  preSignCheck() {
    return Promise.resolve({ Success: true });
  },

  async _withSigningLoader(message, callback) {
    let modals = this.get('modals');
    modals.showLoading({ message: message || 'Подписание...' });

    let hideLoader = async () => {
      let hideResult = modals.hideLoading(true);
      if (hideResult && hideResult.then) {
        await hideResult;
      }
    };

    try {
      return await callback();
    } finally {
      await hideLoader();
    }
  },

  async _showSigningAlerts(alerts = []) {
    if (!alerts.length) {
      return;
    }

    let modals = this.get('modals');
    for (let message of alerts) {
      if (isBlank(message)) {
        continue;
      }

      await modals.showAlert({ message });
    }
  },

  /**
  * Обновить статус акта после подписания.
  */
  async _updateActStatus(idDeleted) {
    let model = this.get('targetObject.model'),
      self = this;

    if (isNone(idDeleted) && model.get('actStatus.id') === ActStatusEnum.editing) {
      const actStatus = await this.actStatusProvider.getActStatus(ActStatusEnum.signing);
      model.set('actStatus', actStatus);
    }

    let actSigns = model.get(self.get('contentPropertyName')).toArray(),
      allSigns = true;

    for (let i = 0; i < actSigns.length; i++) {
      let sign = actSigns[i];
      if (isNone(idDeleted) || sign.get('id') !== idDeleted) {
        allSigns = allSigns && !isBlank(sign.get('signDate')) && !isBlank(sign.get('signature'));
      }
    }

    if (allSigns && (isNone(idDeleted) || actSigns.length > 1)) {
      const actStatus = await this.actStatusProvider.getActStatus(ActStatusEnum.signed);
      model.set('actStatus', actStatus);
    }

    return await model.save();
  },


  _refreshSigningList() {
    let refreshSigns = this.get('refreshSigns');
    if (!isNone(refreshSigns)) {
      return refreshSigns();
    }
  },

  /**
     * Нажатие на кнопку подтверждения подписи стилусом
     */
  supplySignature: function (imgPk) {
    this.get('signingInternalModel.currentSigning').set('signature', imgPk);
    return this._onSigningAccepted();
  },

  /**
   * Функция, вызываемая при удачной проверке данных подписания. Сохраняет информацию о
   * подписании в модель.
   */
  async _onSigningAccepted() {
    let sign = this.signingInternalModel.get('currentSigning');

    let alerts = [];

    await this._withSigningLoader('Подписание...', async () => {
      sign.set('signDate', new Date());
      await sign.save();

      this.signingInternalModel.set('state', 'normal');

      let updateAlerts = await this._updateActStatus();
      if (Array.isArray(updateAlerts)) {
        alerts = alerts.concat(updateAlerts);
      }

      await Promise.resolve(this._refreshSigningList());
    });

    await this._showSigningAlerts(alerts);
  },

  /**
   * Обработчик изменения состояния модуля.
   */
  internalModelStateChanged() {
    if (this.get('signingInternalModel.state') === 'signingTypeSelection') {
      this.get('modes.runningOnDevice') ? this.set("signingModalMobile", true) : this.$('#' + this.wrapperId).dimmer('show');
    }

    if (this.get('signingInternalModel.state') === 'normal' ||
      this.get('signingInternalModel.state') === 'signerAdding') {
      this.get('modes.runningOnDevice') ? this.set("signingModalMobile", false) : this.$('#' + this.wrapperId).dimmer('hide');
    }
  },

  internalModelStateObserver: function () {
    once(this, 'internalModelStateChanged');
  }.observes('signingInternalModel.state'),

  /**
   * Обработчик выбора сотрудника в окне лукапа при добавлении подписанта.
   */
  internalModelEmployeeChanged: function () {
    once(this, 'handleAddedEmployeeChange');
  }.observes('signingInternalModel.addedEmployee'),

  /**
 * Видимость кнопки добавить подписанта.
 */
  addButtonDisabled: computed('targetObject.model.status', 'hideAllButtons', function () {
    let status = this.get('targetObject.model.actStatus.id');
    return this.get('hideAllButtons') || !(status === ActStatusEnum.editing ||
      status === ActStatusEnum.signing);
  }),

  /**
  * Обработчик выбора сотрудника в окне лукапа при добавлении подписанта.
  */
  async handleAddedEmployeeChange() {
    let modals = this.get('modals'),
      model = this.get('targetObject.model'),
      poe = this.get('signingInternalModel.addedEmployee');
    if (poe && this.get('signingInternalModel.state') === 'signerAdding') {
      let signsCollection = model.get(this.contentPropertyName),
        self = this,
        signRole = await self.signRoleProvider.getSignRole(self.get('signingInternalModel.role')),
        employee = poe.get('employee'),
        addingFunction = async function () {
          let signData = {
            employee: employee,
            act: model,
            signRole: signRole,
          };
          let store = self.get('store'),
            newSigning = store.createRecord(self.signModelName, signData);
          signsCollection.addObject(newSigning);
          modals.showLoading({ message: 'Добавление подписанта...' });
          newSigning.set('fio', employee.get('fio'));
          newSigning.set('positionName', poe.get('position.name'));
          newSigning.set('partnerName', poe.get('organization.name'));
          self.addSign(newSigning).finally(function () {
            modals.hideLoading(true);
            if (model.get('actStatus.id') !== ActStatusEnum.created) {
              self.send('refreshList');
            }
          });
        };
      if (signsCollection.any(function (item) { return item.get('employee.id') === employee.get('id'); })) {
        const ok = await modals.showConfirmation({
          title: 'Внимание',
          message: 'Выбранный участник уже существует. Добавить его повторно?'
        }).catch(() => false);
        if (ok) {
          addingFunction();
        } else {
          self.set('signingInternalModel.addedEmployee', null);
        }
      } else {
        addingFunction();
      }
    }
  },

  async addSign(newSigning) {
    let self = this,
      modals = self.get('modals'),
      model = this.get('targetObject.model'),
      signsCollection = model.get(this.contentPropertyName);

    signsCollection.addObject(newSigning);
    self.set('signingInternalModel.state', 'normal');
    self.set('signingInternalModel.role', null);

    // Затрем выбранное в лукапе значение, что бы после удаления можно было повторно добавить его же.
    // Иначе обработчик не срабатывает, ведь значение не меняется.
    self.set('signingInternalModel.addedEmployee', null);

    // Статус акта.
    let status = model.get('actStatus.id');

    modals.hideLoading(true);
    // Если уже был "Подписан", то после добавления станет "На подписании".
    if (status === ActStatusEnum.signed) {
      const actStatus = await this.actStatusProvider.getActStatus(ActStatusEnum.signing);
      model.set('actStatus', actStatus);
      return model.save();
    }
    if (status !== ActStatusEnum.created) {
      modals.showLoading({ message: 'Сохранение подписанта...' });
      return newSigning.save().then(() => self.save()).finally(function () {
        modals.hideLoading(true);
      });
    }
    return Promise.resolve();
  },
  
  roleString: function (roleId) {
    switch (roleId) {
      case SignRoleEnum.commissionChair:
        return 'Председатель комиссии (Контролирующий орган)';
      case SignRoleEnum.commissionMember:
        return 'Член комисии';
      case SignRoleEnum.commissionCreator:
        return 'орган, образовавший комиссию в муниципальном округе. Его роль "Представитель администрации';
      default:
        return 'базовый подписант';
    }
  },

  actions: {
    /**
   * Очистка подписантов.
   */
    async clearSigning() {
      const model = this.get('targetObject.model');
      try {
        await this._withSigningLoader('Отмена подписания...', async () => {
          await this.get('actSignProvider').cancelSigning(this.signModelName, model.get('id'));
          await model.reload();
          const signs = model.get(this.contentPropertyName);
          if (signs && signs.reload) {
            await signs.reload();
          }
          await Promise.resolve(this._refreshSigningList());
        });
      } catch (error) {
        await this._showSigningAlerts(['Не удалось отменить подписание. Обновите документ и проверьте права доступа.']);
      }
    },

    async addSignClick(role) {
      this.set('signingInternalModel.role', role);
      let limit = new SimplePredicate('id', FilterOperator.Eq, 'null'),
        organization = this.get('targetObject.model.organization'),
        awp = this.get('targetObject.model.awp'),
        employees = [];
      if (role == SignRoleEnum.TSOemployee) { // для роли ТСО ограничение по типу организации
        limit = new ComplexPredicate(
          Condition.And,
          new SimplePredicate('organization.organizationType', FilterOperator.Eq, OrganizationTypeEnum.TSO),
          new SimplePredicate('actuality', FilterOperator.Eq, true));
      } else {
        if (role == SignRoleEnum.commissionMember) { // для роли Член комиссии ограничение по сотрудникам комиссии
          let committee = await this.admissionCommitteeProvider.getApprovedCommittee(organization, awp);
          if (!isNone(committee)) {
            employees = committee.get('admissionCommitteeEmployee');
            let prs = [];
            employees.forEach(function (element) {
              let employee = element.get('employee');
              if (!isNone(employee)) {
                prs.push(new SimplePredicate('employee', FilterOperator.Eq, employee.get('id')));
              }
            }, this);
            if (prs.length > 0) {
              limit = new ComplexPredicate(Condition.And, (prs.length === 1 ? prs[0] : new ComplexPredicate(Condition.Or, ...prs)),
                new SimplePredicate('actuality', FilterOperator.Eq, true));
            }
          }
        } else { // по остальным ролям ограничение по МО организации
          let orgMO = isNone(organization.get('hierarchy')) ? organization : organization.get('hierarchy');
          limit = new ComplexPredicate(Condition.And,
            new ComplexPredicate(Condition.Or,
              new SimplePredicate('organization', FilterOperator.Eq, orgMO.get('id')),
              new SimplePredicate('organization.hierarchy', FilterOperator.Eq, orgMO.get('id'))),
            new SimplePredicate('actuality', FilterOperator.Eq, true));
        }
      }

      let chooseData = {
        modelToLookup: this.get('signingInternalModel'),
        projection: 'EmployeeAdmissionCommitteeL',
        relationName: 'addedEmployee',
        predicate: limit,
        title: 'Выбор сотрудника',
        modalDialogSettings: {},
        componentName: 'employeeLookup'
      };

      this.set('signingInternalModel.state', 'signerAdding');
      this.sendAction('showLookupDialog', chooseData);
    },
    /**
   * Готов к подписанию. Подписать базового подписанта.
   */
    signInspector() {
      let self = this,
        model = this.get('targetObject.model');
      return model.save().then(() => {
        let modals = self.get('modals');

        model.validate().then(async () => {
          let thisInspectorId = this.get('session.data.authenticated.employee');
          if (isBlank(thisInspectorId)) {
            modals.showAlert({ message: 'Ошибка авторизации пользователя. Выполните выход и вход в приложение повторно.' });
            return;
          }

          let preSignCheckResult = await self.preSignCheck();
          if (!preSignCheckResult.Success) {
            modals.showAlert({ message: preSignCheckResult.Message });
            return;
          }

          let recordArray = model.get(self.get('contentPropertyName')).toArray();
          let inspectorRecord = recordArray.find(record => thisInspectorId === record.get('employee.id')); 

          if (!isNone(this.get('baseSignRole'))) {
            inspectorRecord = recordArray.find(record => thisInspectorId === record.get('employee.id')
              && record.get('signRole.id') === this.get('baseSignRole'));

            if (!inspectorRecord) {
              modals.showAlert({ message: 'Инициировать подписание может только ' + this.roleString(this.get('baseSignRole')) });
              return;
            }
          } else {
            if (!inspectorRecord) {
              modals.showAlert({ message: 'Сотрудник отсутствует в списке подписантов' });
              return;
            }
          }
          let employee = inspectorRecord.get('employee');
          const existingSignature = await this.signatureProvider
            .findSignature(employee);
          let sign = existingSignature && existingSignature.get('signature');

          if (!sign) {
            modals.showAlert({ message: 'У сотрудника отсутствует подпись в справочнике «Сотрудники»' });
            return;
          }

          inspectorRecord.set('signature', sign);
          inspectorRecord.set('signDate', new Date());

          let alerts = [];

          await self._withSigningLoader('Подписание...', async () => {
            await inspectorRecord.save();
            await self._updateActStatus();
            await Promise.resolve(self._refreshSigningList());
          });

          await self._showSigningAlerts(alerts);
        })
          .catch(() => {
            modals.showAlert({ message: 'Заполните обязательные поля.' });
          });
      });
    },

    /**
     * Нажатие на строку списка подписантов.
     *
     * @param {Object} record Модель выбранной строки.
     * @param {Object} params Параметры нажатия, формируемые object-list-view.
     */
    objectListViewRowClick: function (record, params) {
      if (!this.hideAllButtons && (isNone(params.originalEvent)
        || params.originalEvent.target.tagName.toLowerCase() !== 'input')) {
        if (!isBlank(record.get('signDate')) && !isBlank(record.get('signature'))) {
          return;
        }

        // Тут мы смотрим состояние акта.
        let status = this.get('targetObject.model.actStatus.id');
        if (status !== ActStatusEnum.signing) {
          // Иначе подписывать можно только в состоянии "На подписании".
          return;
        }

        if (this.get('session.data.authenticated.employee') !== record.get('employee.id')) {
          // Подписание чужой записи запрещено и повторно проверяется сервером.
          return;
        }

        this.get('signingInternalModel').set('currentSigning', record);
        this.set('signingInternalModel.state', 'signingTypeSelection');
      }
    },

    async beforeDeleteRecord(record, data) {
      data.cancel = true;
      let self = this,
        modals = self.get('modals');
      modals.showLoading({ message: 'Удаление подписанта...' });
      return this._updateActStatus(record.get('id')).finally(function () {
        record.destroyRecord();
        modals.hideLoading(true);
      });
    },

    /**
  * Функция для настройки строк object-list-view.
  *
  * @param {Object} rowConfig Объект с настройками строки.
  * @param {Object} record Модель, соответствующая строке.
  */
    configOlvRowCallback(rowConfig, record) {
      let readonlyColumns = [];

      readonlyColumns.push('signRole.name');
      readonlyColumns.push('fio');
      readonlyColumns.push('positionName');
      readonlyColumns.push('partnerName');
      readonlyColumns.push('signature');
      readonlyColumns.push('signDate');

      set(rowConfig, 'readonlyColumns', readonlyColumns);

      if (isBlank(record.get('signDate')) || isBlank(record.get('signature'))) {
        set(rowConfig, 'canBeDeleted', !this.hideAllButtons);
      } else {
        set(rowConfig, 'canBeDeleted', false);
        set(rowConfig, 'customClass', 'object-list-view-green-row');
      }

      let status = this.get('targetObject.model.actStatus');
      if (status !== ActStatusEnum.editing && status !== ActStatusEnum.created) {
        set(rowConfig, 'canBeDeleted', false);
      }

      // Заголовок мобильной карточки
      set(rowConfig, 'titlePropName', 'signRole.name');
    },
    stilusSigningButtonClick() {
      this.set('stilusSigningVisible', !this.stilusSigningVisible);
    },

    afterShowStilusSigningModal() {
      this.set('signingModalShowed', true);
    },

    afterHideStilusSigningModal() {
      this.set('signingModalShowed', false);
      this.set('stilusSigningVisible', false);
    },

    stilusSignClick: function (imgPk) {
      const signingPromise = resolve(this.supplySignature(imgPk));
      const finish = () => {
        if (this.isDestroying || this.isDestroyed) {
          return;
        }

        this.set('signingModalShowed', false);
        this.set('stilusSigningVisible', false);
      };

      return signingPromise.finally(finish);
    },
    /**
    * Нажатие на кнопку отмены подписи.
    */
    cancelSigningButtonClick() {
      this.get('modes.runningOnDevice') ? this.set("signingModalMobile", false) : this.$('#' + this.wrapperId).dimmer('hide');
      this.set('signingInternalModel.state', 'normal');
    },
  }
});
