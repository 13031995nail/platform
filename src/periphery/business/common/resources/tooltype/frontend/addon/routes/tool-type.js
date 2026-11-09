import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'ToolTypeE',
  modelName: 'tool-type',

  masterTitle: t('forms.tool-type.caption'),
});
