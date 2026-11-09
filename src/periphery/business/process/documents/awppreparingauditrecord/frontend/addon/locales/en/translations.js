import $ from 'jquery';
import AwpPreparingAuditRecordsForm from './forms/awp-preparing-audit-records';
import AwpPreparingAuditRecordForm from './forms/awp-preparing-audit-record';
import AwpPreparingAuditRecord from './models/awp-preparing-audit-record';

const translations = {};

$.extend(true, translations, {
  models: {
    'awp-preparing-audit-record': AwpPreparingAuditRecord,
  },
  forms: {
    'awp-preparing-audit-record': AwpPreparingAuditRecordForm,
    'awp-preparing-audit-records': AwpPreparingAuditRecordsForm,
  }
});

export default translations;
