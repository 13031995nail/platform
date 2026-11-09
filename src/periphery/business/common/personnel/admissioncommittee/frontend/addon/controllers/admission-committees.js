import ListFormController from '@timepath/tmobis-core/controllers/base/actual-object-list-form-common';
import UponListFilterController from '@timepath/tmobis-core/mixins/upon-list-filter-controller';

export default ListFormController.extend(UponListFilterController, {
  editFormRoute: 'admission-committee',

  recordsModelName: undefined,

  actions: {
    removeLookupValue(updateData) {
      this._super(...arguments);
      const isAwpChanged = updateData.relationName === 'awp';
      if (isAwpChanged) {
        this.send('refresh');
      }
    },

    updateLookupValue(updateData) {
      this._super(...arguments);
      const isAwpChanged = updateData.relationName === 'awp';
      if (isAwpChanged) {
        this.send('refresh');
      }
    }
  }
});
