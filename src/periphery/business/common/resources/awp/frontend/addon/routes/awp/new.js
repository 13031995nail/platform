import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'AwpE',
  modelName: 'awp',
  templateName: 'awp',

  masterTitle: t('forms.awp.caption'),
});
