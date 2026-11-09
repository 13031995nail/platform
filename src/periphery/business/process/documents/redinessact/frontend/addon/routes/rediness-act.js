import EditFormRoute from '@timepath/tmobis-business-process-acts-base-act/routes/document-edit-base';
import { translationMacro as t } from 'ember-i18n';
import { computed } from '@ember/object';

export default EditFormRoute.extend({
  modelProjection: 'RedinessActE',
  modelName: 'rediness-act',

  masterTitle: t('forms.rediness-act.caption'),

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
