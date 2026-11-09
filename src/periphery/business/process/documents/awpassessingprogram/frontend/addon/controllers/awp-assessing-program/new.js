import AwpAssessingController from '../awp-assessing-program';
import { once } from '@ember/runloop';
// eslint-disable-next-line ember/no-observers
import { observer } from '@ember/object';
import { inject as service } from '@ember/service';

export default AwpAssessingController.extend({
  committeeProvider: service('admission-committee-provider'),
  awpAssessingProgramProvider: service('awp-assessing-program'),

  // eslint-disable-next-line ember/no-observers
  committeeObserver: observer('model.organization', 'model.awp', function () {
    once(this, 'findCommittee');
  }),

  async findCommittee() {
    const awp = this.get('model.awp');
    const organization = this.get('model.organization');

    if (!awp || !organization)
      return;

    const committee = await this.committeeProvider
      .getCommittee(organization, awp);
    if (!committee)
      return;

    this.set('model.committee', committee);
    this.set('model.assignee', committee.get('leader.employee'));
  },

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

    const existingAct = await this.awpAssessingProgramProvider
      .findAct(organization, awp);
    return existingAct;
  },
});