import $ from 'jquery';

import VerificationRequierementLForm from './forms/verification-requierements';
import VerificationRequierementEForm from './forms/verification-requierement';
import VerificationRequierementModel from './models/verification-requierement';

const translations = {};
$.extend(true, translations, {
  models: {
    'verification-requierement': VerificationRequierementModel,
  },
  forms: {
    'verification-requierements': VerificationRequierementLForm,
    'verification-requierement': VerificationRequierementEForm,
  }
});

export default translations;
