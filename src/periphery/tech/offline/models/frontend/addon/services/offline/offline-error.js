import Service, { inject as service } from '@ember/service';

export default Service.extend({
  modals: service(),
  i18n: service(),

  offlineMessage(messages, label, detail) {
    const i18n = this.get('i18n'),
          modals = this.get('modals'),
          message = i18n.t(
            'features.offline.loadOffline.failedGeneralRecommendations');

    if (messages) {
      messages.push(message);
    } else {
      const fullMessage = message + `\n${label}` + detail;
      modals.showAlert({message: fullMessage});
    }
  }
});
