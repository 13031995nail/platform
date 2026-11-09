import FlexberryBaseComponent from 'ember-flexberry/components/flexberry-base-component';
import { isNone } from '@ember/utils';

export default FlexberryBaseComponent.extend({
  init() {
    this._super(...arguments);

    let placeOfEmployments = this.get('placeOfEmployments');
    let relPlaceOfEmployment = this.get('relatedModel.placeOfEmployment');

    if (!isNone(placeOfEmployments) && placeOfEmployments.length > 0) {
      placeOfEmployments.forEach(place => {
        place.set('employee', this.get('relatedModel'));
        relPlaceOfEmployment.pushObject(place);
      })
    }
  },
});
