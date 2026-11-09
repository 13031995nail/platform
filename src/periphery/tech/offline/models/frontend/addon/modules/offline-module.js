import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";
import { isEmpty } from '@ember/utils';
import { getOwner } from '@ember/application';

/**
 * Ember модуль офлайн-режима.
 * Содержит модели, backup/sync-инфраструктуру и расширения UI.
 */
export class OfflineModule extends ModuleBase {
  getListFormExtensions() {
    return {
      type: 'dictionary',
      customButtons: [
        {
          buttonClasses: '',
          buttonName: 'Перезагрузить справочник',
          iconClasses: 'refresh-cw',
          buttonAction: 'reloadDictionary',
          disabled: false
        }
      ],
      actions: {
        reloadDictionary() {
          const modelName = this.get('model.modelName'),
            offlineBackupManager = this.get('offlineBackupManager'),
            modals = this.get('modals');
          if (isEmpty(offlineBackupManager) || isEmpty(modals)) {
            throw new Error('OfflineModule not loaded. Check your \'modules\' service.')
          }
          let result = offlineBackupManager.reloadDict(modelName, this);
          if (result.success) {
            modals.showAlert({
              title: 'Перезагрузка справочников',
              message: result.message
            });
          }
          else {
            modals.showAlert({
              title: 'Ошибка при перезагрузке справочников',
              message: result.message
            });
          }
        }
      }
    }
  }
};
