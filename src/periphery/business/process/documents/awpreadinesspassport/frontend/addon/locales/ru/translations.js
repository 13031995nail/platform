import $ from 'jquery';
import AwpReadinessPassportsForm from './forms/awp-readiness-passports';
import AwpReadinessPassportForm from './forms/awp-readiness-passport';
import AwpReadinessPassport from './models/awp-readiness-passport';

const translations = {};

$.extend(true, translations, {
  models: {
    'awp-readiness-passport': AwpReadinessPassport,
  },
  forms: {
    'awp-readiness-passport': AwpReadinessPassportForm,
    'awp-readiness-passports': AwpReadinessPassportsForm,
  }
});

export default translations;
