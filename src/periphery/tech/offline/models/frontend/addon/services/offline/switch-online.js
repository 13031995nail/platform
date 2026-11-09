import moment from 'moment';
import config from 'ember-get-config';
import { later } from '@ember/runloop';
import Service, { inject as service } from '@ember/service';

export default Service.extend({
  dexie: service(),
  store: service(),
  modals: service(),
  session: service(),
  network: service(),
  offlineUser: service('offline/offline-user'),
  offlineState: service('offline-globals'),
  offlineEntities: service('offline/offline-entities'),
  syncer: service(),

  async saveRecordsOnline(skipConfirm) {
    const self = this,
          syncer = this.syncer,
          offlineState = this.offlineState;

    const hasNetwork = await this.network.checkNetwork();
    if (!hasNetwork) {
      offlineState.set('checkedToggleOnline', offlineState.get('isOnline'));
      await self.catchNetworkError('Отсутствует подключение к сети. Переход в онлайн-режим невозможен.');
      return;
    }

    try {
      let needSwitch = skipConfirm;
      if (!skipConfirm) {
        const confirm = await this.modals.showConfirmation({
          title: 'Вы действительно хотите перейти в онлайн-режим?',
          message: 'Выгрузка данных может занять длительное время.',
          returnResult: true
        });
        needSwitch = confirm;
      }

      if (!needSwitch) {
        offlineState.set('checkedToggleOnline', false);
        return;
      }

      self.set('syncUpProgressDialogIsVisible', true);

      await offlineState.toggleOnline();
      await this.offlineUser.setUserOnlineState(true);
      await syncer.syncUp(undefined, {
        continueOnError: true,
        useBatchUpdate: true
      });

      self.set('syncUpProgressDialogIsVisible', false);

      await this.modals.showAlert({
        title: 'Приложение работает в онлайн-режиме',
        message: 'Данные успешно сохранены на сервер'
      });
    } catch (ex) {
      await this.catchNetworkError();
      self.set('syncUpProgressDialogIsVisible', false);
    }
  },

  /**
   * Возвращение в оффлайн при ошибке
   * @param {*} message - сообщение об ошибке
   */
  async catchNetworkError(message) {
    await this.offlineState.toggleOffline();
    this.modals.showAlert({
      message: message || 'Выгрузка данных не завершена. Повторите попытку.'});
    // this.set('syncUpProgressDialogIsVisible', false);
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
