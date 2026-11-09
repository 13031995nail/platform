import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'WorkTypeE',
  modelName: 'work-type',
  templateName: 'work-type',
  controllerName: 'work-type',

  masterTitle: t('forms.work-type.caption'),
});
