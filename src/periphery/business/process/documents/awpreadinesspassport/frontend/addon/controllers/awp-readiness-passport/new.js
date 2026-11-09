import RedinessActController from '../awp-readiness-passport';
import { once } from '@ember/runloop';
// eslint-disable-next-line ember/no-observers
import { observer } from '@ember/object';
import { inject as service } from '@ember/service';

export default RedinessActController.extend({
  readinessActProvider: service('readiness-act'),
  readinessPassportProvider: service('awp-readiness-passport'),
  queryParams: ['organizationId', 'awpId'],
  organizationId: null,
  awpId: null,

  // eslint-disable-next-line ember/no-observers
  readinessActObserver: observer('model.organization', 'model.awp', function () {
    once(this, 'findReadinessAct');
  }),

  async customPresave() {
    const existingAct = await this.findExistingAct();
    return Promise.resolve(this.createSaveResult(
      !existingAct, 'Найден существующий акт для указанных данных.'));
  },

  async findExistingAct() {
    const awp = this.get('model.awp');
    const organization = this.get('model.organization');   

    if (!awp || !organization)
      return;

    const existingAct = await this.readinessPassportProvider.findAct(organization, awp);
    return existingAct;
  },

  async findReadinessAct() {
    const awp = this.get('model.awp');
    const organization = this.get('model.organization');   

    if (!awp || !organization)
      return;

    const readinessAct = await this.readinessActProvider.findAct(organization, awp);
    this.set('model.readinessAct', readinessAct);
  }    
});
