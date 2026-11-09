import $ from 'jquery';
import moment from 'moment';
import { Promise } from 'rsvp';
import config from 'ember-get-config';
import { later } from '@ember/runloop';
import { isEmpty } from '@ember/utils';
import { computed } from '@ember/object';
import { getOwner } from '@ember/application';
import Service, { inject as service } from '@ember/service';
import QueryBuilder from 'ember-flexberry-data/query/builder';

export default Service.extend({
  dexie: service(),
  store: service(),
  modals: service(),
  session: service(),
  network: service(),
  offlineUser: service('offline/offline-user'),
  offlineState: service('offline-globals'),
  offlineEntities: service('offline/offline-entities'),

  _queueSyncDownWorksCount: computed('dexie.queueSyncDownWorksCount', function () {
    return this.get('dexie.queueSyncDownWorksCount');
  }),

  syncDownProgress: 0,
  syncDownOverallProgress: 0,
  syncDownStatusCaption: '',
  syncDownProgressDialogIsVisible: false,
  
  connectTimeoutTime: undefined,

  syncDownProgressChanged: function () {
    $('#syncDownProgress').progress({
      percent: this.syncDownProgress
    });
  }.observes('syncDownProgress'),

  syncDownOverallProgressChanged: function () {
    $('#syncDownOverallProgress').progress({
      percent: this.syncDownOverallProgress
    });
  }.observes('syncDownOverallProgress'),

  async saveRecordsOffline() {
    let error = false,
        errorTitle = undefined,
        errorMsg = undefined;

    try {
      const offlineState = this.offlineState,
            reqs = await this.getConditionRequirements(),
            conds = await this.checkConditions(reqs);

      if (!conds.satisfies) {
        offlineState.set('checkedToggleOnline', true);
        if (conds.message) {
          error = true;
          errorTitle = conds.title || 'Данные не были сохранены локально';
          errorMsg = conds.message;
        }
      }
      
      if (error) {
        this.showResult({title: errorTitle, message: errorMsg });
      } else {
        await this.syncRecords(reqs);
      }
    } catch(ex) {
      this.showRetryModal();
    }
  },

  showResult(message) {
    const modals = this.modals;
    // TODO: Modal timeout
    setTimeout(() => {
      modals.showAlert({ 
        title: message.title, 
        message: message.message });
    }, 500);
  },

  async getConditionRequirements() {
    return undefined;
  },

  // eslint-disable-next-line no-unused-vars
  async checkConditions(reqs) {
    const offlineState = this.offlineState;
    return offlineState.get('isOnline');
  },

  async syncRecords(reqs) {
    await this.setPreSwitchStates();
    const dictionaries = await this.getAndClearEntities(reqs);

    try {
      for (let index = 0; index < dictionaries.length; index++) {
        this.set('syncDownOverallProgress', 
          index * 100 / dictionaries.length);
        this.set('syncDownStatusCaption', 
          'Загрузка "' + dictionaries[index].caption + '"');
        await this.syncEntity(
          dictionaries[index].modelName,
          dictionaries[index].projection,
          dictionaries[index].predicate,
          dictionaries[index].projection1,
          dictionaries[index].predicate1);
      }

      await this.setSuccessSwitchStates();
      await this.offlineState.toggleOffline();
      await this.offlineUser.setUserOnlineState(false);
      await this.onSwitchSuccess();
      await this.setPostSwitchStates();
    } catch (ex) {
      this.showRetryModal();
      // await this.onSwitchError();
      console.error(ex);
    } finally {
      // await this.setPostSwitchStates();
    }
  },

  async setPreSwitchStates() {
    const offlineState = this.offlineState;
    this.set('syncDownProgressDialogIsVisible', true);
    this.set('appButtonsVisible', false);
    this.set('syncDownOverallProgress', 0);
    offlineState.set('isSyncDownWhenOnlineEnabled', false);
    offlineState.set('synchronousSyncDownEnabled', true);
  },

  async setSuccessSwitchStates() {
    this.set('syncDownProgressDialogIsVisible', false);
    this.set('appButtonsVisible', true);
  },

  async getAndClearEntities(reqs) {
    const dictionaries = await this.offlineEntities
      .getConditionalEntities(reqs);
    this.clearExistingModels(dictionaries);
    return dictionaries;
  },

  async onSwitchSuccess() {
    const modals = this.modals;
    modals.showAlert({ 
          title: 'Данные успешно сохранены локально', 
          message: 'Приложение работает в оффлайн-режиме' });
  },

  async onSwitchError() {
    const offlineState = this.offlineState;
    const isOnline = await this.get('network').checkNetwork();
    offlineState.set('checkedToggleOnline', true);
    await this.showSwitchError(isOnline);
    this.set('syncDownProgressDialogIsVisible', false);
    this.set('appButtonsVisible', true);
  },

  async showSwitchError(isOnline) {
    const modals = this.modals;
    let message = (isOnline)
      ? 'Возможные причины:\n' +
        '1. Слабый сигнал интернета\n' +
        '2. Недостаточно памяти, очистите КЭШ'
      : 'Загрузка данных не завершена по причине отсутствия связи. Повторите действие по переходу в оффлайн.';
    
    modals.showAlert({
      title: 'Ошибка при переходе в офлайн режим',
      message: message
    });
  },

  async setPostSwitchStates() {
    const offlineState = this.offlineState;
    offlineState.set('synchronousSyncDownEnabled', false);
    offlineState.set('isSyncDownWhenOnlineEnabled', 
      config.APP.offline.syncDownWhenOnlineEnabled);
  },

  /**
   * Очистить существующие модели в ЛБД.
   */
  clearExistingModels(dictionaries) {
    const self = this,
          clearPromises = [];
    dictionaries.forEach(function (dictionary) {
      let modelName = dictionary.modelName;
      clearPromises.push(self.clearExistingModel(modelName));
    });

    clearPromises.push(self.clearExistingModel('i-c-s-soft-s-t-o-r-m-n-e-t-business-audit-objects-audit-field'));
    clearPromises.push(self.clearExistingModel('i-c-s-soft-s-t-o-r-m-n-e-t-business-audit-objects-audit-entity'));

    return Promise.all(clearPromises);
  },

  clearExistingModel(modelName) {
    const self = this,
          offlineEntities = this.offlineEntities,
          clearIgnoreEntities = offlineEntities.clearIgnoreEntities,
          localStore = getOwner(self).lookup('store:local'),
          model = localStore.modelFor(modelName),
          hasManyRels = []; 

    model.eachRelationship((name, attrs) => {
      if (attrs.kind === 'hasMany') {
        hasManyRels.pushObject(attrs.type);
      }
    });

    let promises = [];
    hasManyRels.forEach(x => {
      if (!clearIgnoreEntities.includes(x)) {
        let localAdapter = localStore.adapterFor(x);
        promises.push(localAdapter.clear(x));
      }
    });

    return Promise.all(promises).then(() => {
      let localAdapter = localStore.adapterFor(modelName);
      return localAdapter.clear(modelName);
    });
  },

  async syncEntity(modelName,
    projectionName, predicate,
    projectionName1, predicate1) {
    const store = this.store;
    this.set('syncDownProgress', 0);

    let queryBuilder = new QueryBuilder(store)
      .from(modelName)
      .selectByProjection(projectionName)
      .count();

    if (predicate) {
      queryBuilder = queryBuilder.where(predicate);
    }

    // Загружаем по 50 записей за один запрос, начиная с 0 позиции.
    await this.syncEntityRange(modelName, 0, 50, queryBuilder);
    if (!isEmpty(projectionName1)) {
      let queryBuilder1 = new QueryBuilder(store)
        .from(modelName)
        .selectByProjection(projectionName1)
        .where(predicate1)
        .count();
      await this.syncEntityRange(modelName, 0, 50, queryBuilder1);
    }

    return true;
  },

  async syncEntityRange(modelName, position, number, queryBuilder) {
    const store = this.store,
          query = queryBuilder.skip(position).top(number).build();
    query.useOnlineStore = true;

    const result = await store.query(modelName, query);
    position += number;
    let totalRecordsCount = result.get('meta.count'),
        progress = position * 100 / totalRecordsCount;
    progress = progress > 100 ? 100 : progress;
    this.set('syncDownProgress', progress);

    if (position < totalRecordsCount) {
      return this.syncEntityRange(modelName, position, number, queryBuilder);
    }

    return true;
  },

  async showRetryModal() {
    const self = this,
          modals = this.modals,
          offlineState = this.offlineState;

    try {
      await modals.showConfirmation({
          title: 'Желаете остановить загрузку?',
          message: 'При выгрузке данных в локальное хранилище была потеряна связь с интернетом'});
      
      this.showResult({
        title: 'Данные не были сохранены локально',
        message: 'Ошибка при выгрузке' });
      offlineState.set('checkedToggleOnline', true);
      await this.setPostSwitchStates();
    } catch (_) {
      self.startPeriodicOnlineCheck();
    }
  },
  
  startPeriodicOnlineCheck() {
    const timeout = moment().add(2, 'm');
    this.set('connectTimeoutTime', timeout);
    later(this, this.periodicOnlineChecked, config.APP.offline.periodicCheckStep);
  },
  
  async periodicOnlineChecked() {
    const network = this.network,
          connectTimeoutTime = this.get('connectTimeoutTime');

    const result = await network.checkNetwork();
    if (result) {
      this.saveRecordsOffline();
    } else {
      if (connectTimeoutTime > moment()) {
        later(this, this.periodicOnlineChecked,
          config.APP.offline.periodicCheckStep);
      } else {
        this.showRetryModal();
      }
    }
  },
});
