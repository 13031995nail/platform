import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'VerificationRequierementE',
  modelName: 'verification-requierement',

  masterTitle: t('forms.verification-requierement.caption'),
});
