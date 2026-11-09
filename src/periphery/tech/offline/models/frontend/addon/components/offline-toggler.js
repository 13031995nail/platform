import $ from 'jquery';
import Component from '@ember/component';
import { computed } from '@ember/object';
import { inject as service } from '@ember/service';

export default Component.extend({
  modals: service(),
  offlineState: service('offline-globals'),
  switchOffline: service('offline/switch-offline'),
  switchOnline: service('offline/switch-online'),

  onlineStatus: computed('offlineState.isOnline', function () {
    let isOnline = this.get('offlineState.isOnline');
    return isOnline ? 'online' : 'offline';
  }),

  offlineMode: computed(
    'session.isAuthenticated',
    'offlineState.offlineModeAvailable', function () {
    return this.get('offlineState.offlineModeAvailable');
  }),

  actions: {
    async toggleWork() {
      const modals = this.modals,
            offlineState = this.offlineState,
            isOnline = this.get('offlineState.isOnline');

      $('.ui.sidebar.left').sidebar('hide');

      if (isOnline) {
        // Переход из онлайн в оффлайн
        const result = await modals.showConfirmation({ 
          message: 'Вы действительно хотите перейти в оффлайн-режим?',
          returnResult: true
        });
        
        if (!result) {
          offlineState.set('checkedToggleOnline', true);
          return;
        }
        
        await this.switchOffline.saveRecordsOffline();
        this.onMovedOffline();
      } else {
        // Переход из оффлайн в онлайн
        await this.switchOnline.saveRecordsOnline();
        this.onMovedOnline();
      }
    },
  }
});