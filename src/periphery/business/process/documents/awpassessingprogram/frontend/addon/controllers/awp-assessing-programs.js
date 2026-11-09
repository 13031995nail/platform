import ListFormController from '@timepath/tmobis-business-process-acts-base-act/controllers/documents-list-base';
import UponListFilterController from '@timepath/tmobis-core/mixins/upon-list-filter-controller';

export default ListFormController.extend(UponListFilterController, {
  editFormRoute: 'awp-assessing-program',

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
