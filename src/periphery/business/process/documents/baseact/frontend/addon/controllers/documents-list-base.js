import ListFormController from '@timepath/tmobis-core/controllers/base/list-form-common';

import { isNone } from '@ember/utils';

/**
 * Списковая форма документов
 */
export default ListFormController.extend({
  actions: {
    configurateObjectlistviewRow(rowConfig, record) {
      // TODO: скрывает возможность удаления записи из строки
      // TODO: т.к. она не работает
      rowConfig.canBeDeleted = false;

      // Заголовок мобильной карточки
      const cardTitle = !isNone(record.get('name')) ? record.get('name') : record.get('id');
      rowConfig.cardTitle = cardTitle;
    },

    beforeDeleteRecord(record, data) {
      if (!record || record.get('actStatus.deleteProtected')) {
        if (data) {
          data.cancel = true;
        }
      }
    }
  }
});
