import EditFormRoute from '@timepath/tmobis-business-process-acts-base-act/routes/document-edit-base';
import { inject as service } from '@ember/service';
import { translationMacro as t } from 'ember-i18n';
import { computed } from '@ember/object';

export default EditFormRoute.extend({
  employeeProvider: service(),

  modelProjection: 'AwpAssessingProgramE',
  modelName: 'awp-assessing-program',

  masterTitle: t('forms.awp-assessing-program.caption'),

  canSign: undefined,

  async afterModel(model) {
    await this._super(...arguments);
    await this.setCanSign(model);
  },

  setupController(controller) {
    this._super(...arguments);
    controller.set('canSign', this.canSign);
  },

  async setCanSign(model) {
    const currentEmployee = await this.employeeProvider.getCurrentEmployee();
    const assignee = model.get('assignee');
    this.set('canSign', currentEmployee.get('id') == assignee.get('id'));
  },

  developerUserSettings: computed(function () {
    return {
      'signersGroupEdit': {
        'DEFAULT': {
          'sorting': [{ 'propName': 'roleIndex', 'direction': 'asc' }],
          'columnWidths': [
            { 'propName': 'OlvRowToolbar', 'width': 15, 'fixed': true },
            { 'propName': 'OlvRowMenu', 'fixed': true }
          ],
        }
      }
    }
  }),
});
