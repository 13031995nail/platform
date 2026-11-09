import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'PositionE',
  modelName: 'position',

  masterTitle: t('forms.position.caption'),
});
