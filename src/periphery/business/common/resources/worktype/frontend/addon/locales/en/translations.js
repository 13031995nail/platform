import $ from 'jquery';

import WorkTypeForm from './forms/work-types';
import WorkTypeForms from './forms/work-type';
import WorkTypeModel from './models/work-type';

const translations = {};
$.extend(true, translations, {
  models: {
    'work-type': WorkTypeModel,
  },
  forms: {
    'work-type': WorkTypeForm,
    'work-types': WorkTypeForms,
  }
});

export default translations;
