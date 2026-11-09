import FormController from '@timepath/tmobis-core/controllers/base/dictionaries-list-form-common';
import { translationMacro as t } from 'ember-i18n';

export default FormController.extend({
  title: t('forms.dashboards.caption'),
  editFormRoute: 'tmobis-dashboard',
});
