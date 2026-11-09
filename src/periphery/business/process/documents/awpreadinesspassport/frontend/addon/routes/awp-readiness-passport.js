import EditFormRoute from '@timepath/tmobis-business-process-acts-base-act/routes/document-edit-base';
import { computed } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';
import config from 'ember-get-config';

export default EditFormRoute.extend({
  modelProjection: 'AwpReadinessPassportE',
  modelName: 'awp-readiness-passport',

  masterTitle: t('forms.awp-readiness-passport.caption'),

  developerUserSettings: computed(function () {
    return {
      'awp-readiness-passport-objects': {
        'sorting': [{ 'propName': 'number', 'direction': 'asc' }],
        'DEFAULT': {
          'perPage': config.APP.components.flexberryObjectlistview.defaultPerPage,
          'columnWidths': [
            { 'propName': 'OlvRowToolbar', 'width': 15, 'fixed': true },
            { 'propName': 'OlvRowMenu', 'fixed': true }
          ],
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
  }),
});
