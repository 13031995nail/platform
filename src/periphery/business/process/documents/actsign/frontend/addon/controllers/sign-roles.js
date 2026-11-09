import ListFormController
  from '@timepath/tmobis-core/controllers/base/dictionaries-list-form-common';
import { translationMacro as t } from 'ember-i18n';

export default ListFormController.extend({
  title: t('forms.sign-roles.caption'),
  editFormRoute: 'sign-role',
});
