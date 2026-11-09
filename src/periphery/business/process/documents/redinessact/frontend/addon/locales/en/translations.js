import $ from 'jquery';
import RedinessActsForm from './forms/rediness-acts';
import RedinessActForm from './forms/rediness-act';
import RedinessAct from './models/rediness-act';

const translations = {};

$.extend(true, translations, {
  models: {
    'redinessAct': RedinessAct,
  },
  forms: {
    'rediness-act': RedinessActForm,
    'rediness-acts': RedinessActsForm,
  }
});

export default translations;
