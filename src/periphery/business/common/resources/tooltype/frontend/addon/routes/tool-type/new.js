import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'ToolTypeE',
  modelName: 'tool-type',
  templateName: 'tool-type',
  controllerName: 'tool-type',

  masterTitle: t('forms.tool-type.caption'),
});
