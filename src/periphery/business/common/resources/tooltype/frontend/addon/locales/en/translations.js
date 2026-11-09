import $ from 'jquery';

import ToolTypeForm from './forms/tool-type';
import ToolTypeForms from './forms/tool-types';
import ToolTypeModel from './models/tool-type';

const translations = {};
$.extend(true, translations, {
  models: {
    'tool-type': ToolTypeModel,
  },
  forms: {
    'tool-type': ToolTypeForm,
    'tool-types': ToolTypeForms,
  }
});

export default translations;
