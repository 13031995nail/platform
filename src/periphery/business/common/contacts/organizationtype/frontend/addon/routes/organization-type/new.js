import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { translationMacro as t } from 'ember-i18n';

export default EditFormNewRoute.extend({
  modelProjection: 'OrganizationTypeE',
  modelName: 'organization-type',
  templateName: 'organization-type',

  masterTitle: t('forms.organization-type.caption'),
});
