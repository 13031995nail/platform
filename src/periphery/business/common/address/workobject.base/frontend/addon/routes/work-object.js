import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'WorkObjectE',
  modelName: 'work-object',

  masterTitle: t('forms.work-object-e.caption'),
});