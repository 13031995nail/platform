import ListFormRoute from '@timepath/tmobis-core/routes/base/list-form';
import config from 'ember-get-config';
import { computed } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';

export default ListFormRoute.extend({
  modelProjection: 'EvaluationSheetL',
  modelName: 'evaluation-sheet',

  masterTitle: t('forms.evaluation-sheets.caption'),

  developerUserSettings: computed(function () {
    return {
      'objectlistviewComponentName': {
        'DEFAULT': {
          'perPage': config.APP.components.flexberryObjectlistview.defaultPerPage,
          "sorting": [{ "propName": "regDate", "direction": "desc" }],
          'columnWidths': [
            { 'propName': 'OlvRowMenu', 'fixed': true },
            { 'propName': 'OlvRowToolbar', 'fixed': true }
          ]
        }
      }
    }
  }),
});
