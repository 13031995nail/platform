import EditFormController from '@timepath/tmobis-core/controllers/base/edit-form-common';

/**
 * Форма редактирования документов
 */
export default EditFormController.extend({
  actions: {
    refreshSigns() {
      this.send('refreshList');
    }
  }
});
