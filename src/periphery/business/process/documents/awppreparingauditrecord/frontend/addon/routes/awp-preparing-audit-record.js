import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { translationMacro as t } from 'ember-i18n';

export default EditFormRoute.extend({
  modelProjection: 'AwpPreparingAuditRecordE',
  modelName: 'awp-preparing-audit-record',

  masterTitle: t('forms.awp-preparing-audit-record.caption'),
});
