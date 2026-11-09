import DictionariesRoute from '@timepath/tmobis-core/routes/dictionaries';
import { translationMacro as t } from 'ember-i18n';

export default DictionariesRoute.extend({
  masterTitle: t('forms.documents.caption'),
});
