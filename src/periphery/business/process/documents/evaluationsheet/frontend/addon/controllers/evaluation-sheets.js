import ListFormController from '@timepath/tmobis-core/controllers/base/list-form-common';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';

export default ListFormController.extend({
  editFormRoute: 'evaluation-sheet',

  actions: {
    configurateObjectlistviewRow(rowConfig, record) {
      this._super(...arguments);

      const statusId = record && record.get('actStatus.id');
      const canBeDeleted = this.get('listFormOperationsComputed.delete')
        && statusId === ActStatusKeys.editing;

      rowConfig.canBeDeleted = canBeDeleted;
    },

    beforeDeleteRecord(record, data) {
      const statusId = record && record.get('actStatus.id');
      const canBeDeleted = statusId === ActStatusKeys.editing;

      if (!canBeDeleted && data) {
        data.cancel = true;
      }
    }
  }
});
