import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'VerificationRequierementE',
  modelName: 'verification-requierement',
  templateName: 'verification-requierement',

  masterTitle: t('forms.verification-requierement.caption'),
});
