import FlexberryBaseComponent from 'ember-flexberry/components/flexberry-base-component';
import { isNone } from '@ember/utils';
import { computed, observer } from '@ember/object';
import { once } from '@ember/runloop';

export default FlexberryBaseComponent.extend({
  init() {
    this._super(...arguments);
    this.set('valueOptional', this.get('relatedModel.calculatedReq') === 1 ? true : false);
  },

  enteredValue: computed('relatedModel.hasEnteredValue', 'relatedModel.calculatedReq', {
    get() {
      return this.get('relatedModel.hasEnteredValue') ? this.get('relatedModel.calculatedReq') : '';
    },
    set(key, value) {
      return value;
    }
  }),

  valueOptionalObserver: observer('valueOptional', function () {
    once(this, 'valueOptionalChanged');
  }),

  valueOptionalChanged() {
    this.set('relatedModel.calculatedReq', this.get('valueOptional') ? 1 : 0);
    this.saveModelAndRefresh();
  },

  readonlyOptional: computed('relatedModel.supportingDocFile', function () {
    let docFile = this.get('relatedModel.supportingDocFile');
    return !isNone(docFile) ? true : false
  }),

  readonlyNotOptional: computed('relatedModel.supportingDocFile', function () {
    let relatedModel = this.get('relatedModel');
    return relatedModel.get('isOptional') ? false : true
  }),

  saveModelAndRefresh() {
    let relatedModel = this.get('relatedModel'),
      controller = this.get('targetObject.currentController');
    relatedModel.save().then(()=> { 
      controller.send('refreshList'); 
    }).catch((error) => {
      console.error('Ошибка при сохранении:', error);
    });
  },

  actions: {
    onFocusOutCustom() {
      const value = this.get('enteredValue');
      const hasValue = !isNone(value) && String(value).trim() !== '';
      const number = hasValue ? Number(String(value).replace(',', '.')) : 0;
      if (!Number.isFinite(number)) {
        return;
      }
      this.set('relatedModel.hasEnteredValue', hasValue);
      this.set('relatedModel.calculatedReq', number);
      this.saveModelAndRefresh();
    }
  }
})
