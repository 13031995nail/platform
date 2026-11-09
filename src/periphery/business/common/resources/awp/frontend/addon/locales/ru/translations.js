import $ from 'jquery';

import AwpLForm from './forms/awps';
import AwpEForm from './forms/awp';
import AwpModel from './models/awp';

const translations = {};
$.extend(true, translations, {
  models: {
    'awp': AwpModel,
  },
  forms: {
    'awps': AwpLForm,
    'awp': AwpEForm,
  }
});

export default translations;
