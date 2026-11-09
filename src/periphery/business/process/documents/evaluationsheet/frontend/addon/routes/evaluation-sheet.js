import EditFormRoute from '@timepath/tmobis-business-process-acts-base-act/routes/document-edit-base';
import { computed } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';
import config from 'ember-get-config';
import { inject as service } from '@ember/service';

export default EditFormRoute.extend({
  modelProjection: 'EvaluationSheetE',
  modelName: 'evaluation-sheet',

  rolesManager: service(),

  masterTitle: t('forms.evaluation-sheet.caption'),

  setupController(controller) {
    this._super(...arguments);
    const rolesManager = this.get('rolesManager');

    controller.set('enableEdit', rolesManager.isAdmin || rolesManager.hasRole('Потребитель')
      || rolesManager.hasRole('ТСО') || rolesManager.hasRole('Контролирующий орган')
      || rolesManager.hasRole('Член комиссии'));
  },

  developerUserSettings: computed(function () {
    return {
      'evaluation-sheet-requirements': {
        'DEFAULT': {
          'perPage': config.APP.components.flexberryObjectlistview.defaultPerPage,
          'sorting': [{ 'propName': 'sortOrder', 'direction': 'asc' }],
        }
      },
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
  })
});
