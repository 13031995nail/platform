import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'ActTypeE',
  modelName: 'act-type',

  masterTitle: t('forms.act-type-e.caption'),
});
