import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'ActStatusE',
  modelName: 'act-status',
  templateName: 'act-status',

  masterTitle: t('forms.act-status.caption'),
});
