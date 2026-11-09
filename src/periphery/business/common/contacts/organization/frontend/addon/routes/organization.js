import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { computed } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'OrganizationE',
  modelName: 'organization',

  masterTitle: t('forms.organization-e.caption'),

  developerUserSettings: computed(function () {
    return {
      'organizationTypeOrganization': {
        'DEFAULT': {
          'columnWidths': [
            { 'propName': 'shortName', 'width': 150 },
            { 'propName': 'name', 'width': 700 },
          ],
        }
      }
    }
  }),
});
