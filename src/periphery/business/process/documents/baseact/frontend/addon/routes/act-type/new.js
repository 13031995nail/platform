import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'ActTypeE',
  modelName: 'act-type',
  templateName: 'act-type',

  masterTitle: t('forms.act-type-e.caption'),
});
