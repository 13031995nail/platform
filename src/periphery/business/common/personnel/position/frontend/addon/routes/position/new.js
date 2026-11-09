import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'PositionE',
  modelName: 'position',
  templateName: 'position',

  masterTitle: t('forms.position.caption'),
});
