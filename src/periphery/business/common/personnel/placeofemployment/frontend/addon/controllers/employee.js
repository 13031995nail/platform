import EditFormController from '@timepath/tmobis-core/controllers/employee';
import { isEmpty } from '@ember/utils';

export default EditFormController.extend({
  async customPresave() {
    let placeOfEmployment = this.model.placeOfEmployment;
    if (!isEmpty(placeOfEmployment) && placeOfEmployment.length > 0) {
      placeOfEmployment.forEach(pl => {
        pl.set('employee', this.model);
      }) 
    }
    return this._super(arguments);
  },

  getCellComponent: function (attr, bindingPath) {
    let cellComponent = this._super(...arguments);
    let updateLookupValue = this.get('actions.updateLookupValue').bind(this);
    if (attr.kind === 'belongsTo') {
      if (bindingPath === 'organization') {
        cellComponent.componentProperties = {
          choose: 'showLookupDialog',
          chooseText: '...',
          remove: 'removeLookupValue',
          displayAttributeName: 'name',
          required: true,
          relationName: 'organization',
          projection: 'OrganizationL',
          updateLookupValue: updateLookupValue,
          lookupLimitPredicate: this.actualityCustomLimitPredicate
        };
      }
      if (bindingPath === 'position') {
        cellComponent.componentProperties = {
          choose: 'showLookupDialog',
          chooseText: '...',
          remove: 'removeLookupValue',
          displayAttributeName: 'name',
          required: true,
          relationName: 'position',
          projection: 'PositionL',
          updateLookupValue: updateLookupValue,
          lookupLimitPredicate: this.actualityCustomLimitPredicate
        };
      }
    }
    if (bindingPath === 'phone') {
      cellComponent.componentName = 'flexberry-field';
    }
    return cellComponent;
  }
});