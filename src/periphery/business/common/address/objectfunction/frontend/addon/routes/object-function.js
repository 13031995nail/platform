import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'ObjectFunctionE',
  modelName: 'object-function',

  masterTitle: t('forms.object-function-e.caption'),
});
