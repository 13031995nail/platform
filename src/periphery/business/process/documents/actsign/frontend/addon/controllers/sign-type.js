import DictionariesEditFormController from '@timepath/tmobis-core/controllers/base/dictionaries-edit-form-common';
import { translationMacro as t } from 'ember-i18n';

export default DictionariesEditFormController.extend({
  title: t('forms.sign-type.caption'),
  subtitle: t('forms.sign-type.caption'),
  parentRoute: 'sign-types',
});
