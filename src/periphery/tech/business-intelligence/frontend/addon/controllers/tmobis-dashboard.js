import FormController from '@timepath/tmobis-core/controllers/base/dictionaries-edit-form-common';
import { translationMacro as t } from 'ember-i18n';

export default FormController.extend({
  title: t('forms.dashboard.caption'),
  parentRoute: 'tmobis-dashboards',
});
