import ListFormRoute from '@timepath/tmobis-core/routes/base/list-form';
import { computed } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';
import config from 'ember-get-config';

export default ListFormRoute.extend({
  modelProjection: 'AwpL',
  modelName: 'awp',

  masterTitle: t('forms.awps.caption'),

  developerUserSettings: computed(function () {
    return {
      'objectlistviewComponentName': {
        'DEFAULT': {
          'perPage': config.APP.components.flexberryObjectlistview.defaultPerPage,
          'columnWidths': [
            { 'propName': 'OlvRowMenu', 'fixed': true },
            { 'propName': 'OlvRowToolbar', 'fixed': true }
          ]
        }
      }
    }
  }),
});
