import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'ActStatusE',
  modelName: 'act-status',

  masterTitle: t('forms.act-status.caption'),
});
