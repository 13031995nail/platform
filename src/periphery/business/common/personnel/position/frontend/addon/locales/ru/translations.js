import $ from 'jquery';

import PositionLForm from './forms/positions';
import PositionEForm from './forms/position';
import PositionModel from './models/position';

const translations = {};
$.extend(true, translations, {
  models: {
    'position': PositionModel,
  },
  forms: {
    'positions': PositionLForm,
    'position': PositionEForm,
  }
});

export default translations;
