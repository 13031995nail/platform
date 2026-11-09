/* eslint-disable ember/no-observers */
import EditFormController from '@timepath/tmobis-business-process-acts-base-act/controllers/documents-edit-base';
import { computed, observer } from '@ember/object';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import Condition from 'ember-flexberry-data/query/condition';
import { isEmpty, isNone } from '@ember/utils';
import { inject as service } from '@ember/service';
import OrganizationTypeEnum from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';
import SignRoleEnum from '@timepath/tmobis-business-process-acts-actsign/enums/sign-role';
import $ from 'jquery';
import { once } from '@ember/runloop';

const TEXT_ONLY_COLUMNS = Object.freeze([
  'reqNumber',
  'shortReqName',
  'supportingDocName',
  'indicatorName'
]);

const FILE_ATTACHMENT_COLUMN = 'supportingDocFile';
const ENABLE_COMMISSION_ROW_VERIFICATION = false;
const ALLOW_MANUAL_TSO_VERIFICATION_TOGGLE = false;

export default EditFormController.extend({
  actSignProvider: service(),
  httpService: service('http-request'),
  modals: service(),
  rolesManager: service(),
  reports: service(),
  session: service(),

  parentRoute: 'evaluation-sheets',

  enableEdit: false,

  readonlyControls: computed('readonly', 'enableEdit', function () {
    return this.get('readonly') || !this.get('enableEdit');
  }),

  // Предикат для фильтрации актуальных организаций в лукапе
  organizationCustomLimitPredicate: computed(function () {
    return new SimplePredicate('actuality', FilterOperator.Eq, true);
  }),

  // Предикат для фильтрации актуальных ОЗП в лукапе
  awpCustomLimitPredicate: computed(function () {
    return new SimplePredicate('actuality', FilterOperator.Eq, true);
  }),

  // Предикат для фильтрации объектов работ по организации и актуальности
  workObjectLimitPredicate: computed('model.organization', function () {
    const organization = this.get('model.organization');
    if (!organization) {
      return new SimplePredicate('actuality', FilterOperator.Eq, true);
    }
    return new ComplexPredicate(
      Condition.And,
      new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
      new SimplePredicate('actuality', FilterOperator.Eq, true)
    );
  }),

  // Ручное редактирование isVerifiedTSO отключено, но может быть возвращено константой выше.
  canEditIsVerifiedTSO: computed('session.data.authenticated.roles', 'readonly', function () {
    const rolesManager = this.get('rolesManager');
    const isTSO = rolesManager.hasRole('ТСО');
    return ALLOW_MANUAL_TSO_VERIFICATION_TOGGLE && isTSO;
  }),

  // Проверка, является ли тип КА "Прочие"
  isOrganizationTypeOther: computed('model.organization.organizationType.id', function () {
    const organizationType = this.get('model.organization.organizationType');
    if (!organizationType) {
      return false;
    }
    return organizationType.get('id') === OrganizationTypeEnum.Other;
  }),

  customSettingsButtons: computed(function () {
    const buttons = [];
    const reportButtons = this.reports.getReportButtons();
    $.extend(true, buttons, reportButtons);
    return buttons;
  }),

  baseSignRole: computed(function () {
    return SignRoleEnum.commissionMember;
  }),

  isNotNewReadonly: computed('model.isNew', 'readonly', function () {
    return !this.get('model.isNew') || this.get('readonly');
  }),

  rolesToAdd: computed(function () {
    const result = [],
      i18n = this.i18n;
    result.push({
      key: SignRoleEnum.organizationRepresentative,
      name: i18n.t('forms.sign-role.add-organizationRepresentative'),
    });
    if (this.isOrganizationTypeOther) {
      result.push({
        key: SignRoleEnum.TSOemployee,
        name: i18n.t('forms.sign-role.add-TSOemployee'),
      });
    }
    result.push({
      key: SignRoleEnum.commissionMember,
      name: i18n.t('forms.sign-role.add-commissionMember'),
    });
    return result;
  }),

  workObjectChanged: observer('model.workObject', function () {
    once(this, 'onWorkObjectChanged');
  }),

  organizationChanged: observer('model.organization', function () {
    once(this, 'onOrganizationChanged');
  }),

  init() {
    this._super(...arguments);
    this.reports.bindActions(this);
  },

  /**
   * Проверка всех требований перед сохранением и автоматическое проставление isVerifiedTSO
   * Только для типа КА "Прочие"
   * @method customPresave
   * @return {Promise} Результат проверки
   */
  customPresave() {
    const requirements = this.get('model.requirements');
    const isOther = this.get('isOrganizationTypeOther');

    // Автоматическое проставление isVerifiedTSO работает только для типа КА "Прочие"
    if (isOther && requirements && requirements.get('length') > 0) {
      // Проверяем, что все требования имеют isVerified = true
      const allVerified = requirements.every(req => {
        const isVerified = req.get('isVerified');
        return isVerified === true;
      });

      // Автоматически проставляем галочку, если все требования проверены.
      if (allVerified) {
        const currentValue = this.get('model.isVerifiedTSO');
        if (currentValue !== true) {
          this.set('model.isVerifiedTSO', true);
        }
      }
    }

    return this._super(...arguments);
  },

  saveEvaluationSheet() {
    let self = this,
      modals = this.get('modals'),
      model = this.get('model');
    if (model.get('organization.organizationType.id') === OrganizationTypeEnum.Municipality &&
      model.get('isNew')) {
      return modals.showModal({
        message: 'На территории муниципального образования есть объекты тепловых сетей для которых ' +
          'организация, которая будет осуществлять содержание и обслуживание не определена?',
        onApprove: () => {
          model.set('ownerlessObjects', true);
          return self.save().then(() => { self.send('refreshList'); });
        },
        onDeny: () => {
          model.set('ownerlessObjects', false);
          return self.save().then(() => { self.send('refreshList'); });
        }
      });
    } else {
      this.save().then(() => { self.send('refreshList'); });
    }
  },

  onWorkObjectChanged() {
    let model = this.get('model');
    if (model.get('isDeleted')) {
      return;
    }

    let workObj = model.get('workObject'),
      organization = model.get('organization');

    // Если заполняется объект, то Организация должна автоматически проставиться из WorkObject.Organization.
    if (!isNone(workObj)) {
      if (workObj.get('organization') && (isNone(organization) || workObj.get('organization.id') !== model.get('organization.id'))) {
        model.set('organization', workObj.get('organization'));
      }
    }
  },

  onOrganizationChanged() {
    let model = this.get('model');
    if (model.get('isDeleted')) {
      return;
    }

    let workObj = model.get('workObject'),
      organization = model.get('organization');

    // Если организация не соответствует организации объекта, то очищаем объект
    if (!isNone(workObj)) {
      if (workObj.get('organization') && (isNone(organization) || workObj.get('organization.id') !== model.get('organization.id'))) {
        model.set('workObject', undefined);
      }
    }
  },

  getCellComponent(attr, bindingPath, model) {
    let cellComponent = this._super(...arguments);

    const actSignProvider = this.get('actSignProvider');
    const rolesManager = this.get('rolesManager');
    const isTSO = rolesManager.hasRole('ТСО');
    const isCommissionMember = rolesManager.hasRole('Член комиссии');

    if (model && model.modelName === 'evaluation-sheet-sign') {
      cellComponent = actSignProvider.getCellComponent(attr, bindingPath, model, cellComponent);
    } else if (TEXT_ONLY_COLUMNS.includes(bindingPath)) {
      cellComponent.componentName = undefined;
      cellComponent.componentProperties = undefined;
    }

    if (bindingPath === FILE_ATTACHMENT_COLUMN) {
      cellComponent.componentName = 'tmobis-olv-file-evaluation';
      cellComponent.componentProperties = {
        showAddButton: true,
        useSidePageMode: false,
        fileFieldName: bindingPath,
        customSyncFieldValue: 'customSyncFieldValue',
        readonly: this.get('readonlyControls')
      };
    }

    if (bindingPath === 'calculatedReq') {
      cellComponent = {
        componentName: 'tmobis-regex-input-evaluation',
        componentProperties: {
          defaultRegexType: 'decimal20,2',
          inputmode: 'numeric',
          readonly: this.get('readonlyControls')
        }
      };
    }

    // Управление readonly для полей требований ОЛ в зависимости от роли
    // ТСО может редактировать: isCompletedReq (чекбокс), deadline (сроки)
    if (bindingPath === 'isCompletedReq' || bindingPath === 'deadline') {
      if (bindingPath === 'deadline') {
        cellComponent.componentName = 'tmobis-deadline-evaluation';
      }
      if (!cellComponent.componentProperties) {
        cellComponent.componentProperties = {};
      }
      // ТСО может редактировать эти поля
      cellComponent.componentProperties.readonly = this.get('readonly') || !isTSO;
    }

    if (bindingPath === 'problemComment') {
      cellComponent.componentName = 'tmobis-problem-comment-evaluation';
      if (!cellComponent.componentProperties) {
        cellComponent.componentProperties = {};
      }
      // может редактировать это поле ТСО, контролирующий орган, член комиссии, admin
      cellComponent.componentProperties.readonly = this.get('readonly') ||
        !(isTSO || isCommissionMember || rolesManager.hasRole('Контролирующий орган') || rolesManager.isAdmin);
    }

    // isVerified доступно для редактирования в зависимости от типа КА и роли
    // Отображается как чекбокс слева от каждого требования
    if (bindingPath === 'isVerified') {
      const organizationType = this.get('model.organization.organizationType');
      const isOther = organizationType && organizationType.get('id') === OrganizationTypeEnum.Other;
      const isVerifiedTSO = this.get('model.isVerifiedTSO');

      let readonly = true;

      if (isOther) {
        // Для типа КА "Прочие": ТСО может редактировать, если isVerifiedTSO = false
        // Если isVerifiedTSO = true, то readonly для всех (включая ТСО)
        readonly = !isTSO || isVerifiedTSO;
      } else {
        // Для остальных типов КА построчная проверка комиссией отключена,
        // но поведение можно быстро вернуть через константу выше.
        readonly = ENABLE_COMMISSION_ROW_VERIFICATION
          ? !isCommissionMember || !isVerifiedTSO
          : true;
      }

      cellComponent.componentName = 'tmobis-checkbox-evaluation';
      cellComponent.componentProperties = {
        readonly: readonly,
        sizeClass: 'small'
      };
    }

    return cellComponent;
  },

  actions: {
    customSyncFieldValue(relatedModel, value) {
      if (!relatedModel.get('isCalculated')) {
        if (!isEmpty(value)) {
          relatedModel.set('calculatedReq', 1);
        } else {
          if (!relatedModel.get('isOptional')) {
            relatedModel.set('calculatedReq', 0);
          }
        }
      }
    },

    save() {
      return this.saveEvaluationSheet();
    },
  }
});
