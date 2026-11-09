import FlexberryBaseComponent from 'ember-flexberry/components/flexberry-base-component';
import { inject as service } from '@ember/service';
import { isEmpty, isNone } from '@ember/utils';
import { computed } from '@ember/object';
import { once } from '@ember/runloop';

export default FlexberryBaseComponent.extend({
  file: service(),
  modals: service(),
  store: service(),

  stilusSigningVisible: false,

  // eslint-disable-next-line ember/require-return-from-computed
  signatureImage: computed('employeeSignature.signature', function () {
    once(this, 'recalcSignatureImage');
  }),

  async recalcSignatureImage(signatureImagePK) {
    if (isNone(signatureImagePK)) {
      signatureImagePK = this.get('employeeSignature.signature');
    }
    if (signatureImagePK) {
      const file = this.get('file');
      try {
        const signatureImage = await file.getPreviewSrc(signatureImagePK);
        this.set('signatureImage', signatureImage);
      } catch (e) {
        throw new Error(e);
      }
    }
  },

  actions: {
    enlargeSignatureImage() {
      this.set('showSignatureImage', true);
    },
    stilusSigningButtonClick() {
      this.set('stilusSigningVisible', !this.stilusSigningVisible);
    },
    onVisibleStilusSigningModal() {
      this.set('signingModalShowed', true);
    },
    onHiddenStilusSigningModal() {
      this.set('stilusSigningVisible', false);
      this.set('showSignatureImage', false);
      this.set('signingModalShowed', false);
    },
    // Сохраняем новую подпись, показывая прогресс и результат через сервис модальных окон.
    async stilusSignClick(signatureImagePK) {
      const modals = this.modals;
      modals.showLoading({ message: 'Сохранение подписи...' });

      this.set('stilusSigningVisible', false);
      this.set('signingModalShowed', false);

      let employeeSignature = null;
      if (!isEmpty(this.employeeSignature)) {
        employeeSignature = this.employeeSignature;
      } else {
        employeeSignature = this.store.createRecord('employee-signature');
      }
      employeeSignature.set('signature', signatureImagePK);
      employeeSignature.set('employee', this.relatedModel);
      this.recalcSignatureImage(signatureImagePK)

      let alertMessage = 'Подпись успешно сохранена.';

      try {
        await employeeSignature.save();
      } catch (error) {
        alertMessage = 'Не удалось сохранить подпись. Попробуйте еще раз.';
      } finally {
        await modals.hideLoading(true);
      }

      await modals.showAlert({ message: alertMessage });
    },
  }
});
