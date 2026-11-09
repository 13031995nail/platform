import RedinessActController from '../rediness-act';
import { once } from '@ember/runloop';
// eslint-disable-next-line ember/no-observers
import { observer } from '@ember/object';
import { inject as service } from '@ember/service';
import { isNone, isBlank } from '@ember/utils';

export default RedinessActController.extend({
  queryParams: ['organizationId', 'awpId'],
  organizationId: null,
  awpId: null,
  awpAssessingProgramProvider: service('awp-assessing-program'),
  readinessActProvider: service('readiness-act'),

  // eslint-disable-next-line ember/no-observers
  readinessActObserver: observer('model.organization', 'model.awp', function () {
    once(this, 'findAwpAssessingProgram');
  }),

  async customPresave() {
    const existingAct = await this.findExistingAct();
    if (existingAct) {
      return this.createSaveResult(false,
        'По выбранному контрагенту существует неподписанный акт или подписанный акт со статусом Готов/Готов с условиями');
    }

    const awpId = this.get('model.awp.id');
    const organizationId = this.get('model.organization.id');
    const isValidSheets = await this.readinessActProvider
      .checkObjectsEvaluationSheets(organizationId, awpId);

    if (!isValidSheets) {
      return this.createSaveResult(false,
        'Не у всех объектов лица подписаны оценочные листы');
    }

    return this.createSaveResult(true);
  },

  async findExistingAct() {
    const awp = this.get('model.awp');
    const organization = this.get('model.organization');

    if (!awp || !organization)
      return;

    const existingAct = await this.readinessActProvider.findExistingAct(organization, awp);
    return existingAct;
  },

  async findAwpAssessingProgram() {
    const awp = this.get('model.awp');
    const organization = this.get('model.organization');
    let awpAssessingProgram = null;

    if (!isNone(awp) && !isNone(organization)) {
      awpAssessingProgram = await this.awpAssessingProgramProvider.findApprovedAct(organization, awp);
      if (!awpAssessingProgram) {
        if (organization.get('hierarchy')) {
          awpAssessingProgram = await this.awpAssessingProgramProvider.findApprovedAct(organization.get('hierarchy'), awp);
        }
      }
    }

    this.set('model.awpAssessingProgram', awpAssessingProgram);
  }
});
