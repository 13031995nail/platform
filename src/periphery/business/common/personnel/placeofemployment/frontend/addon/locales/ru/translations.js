import $ from 'jquery';
import TmobisPlaceOfEmploymentForm from './forms/place-of-employment';

const translations = {};

$.extend(true, translations, {
    forms: {
        'place-of-employment': TmobisPlaceOfEmploymentForm,
  },
  models: {
    'place-of-employment': {
      validations: {
        organization: {
          __caption__: 'Контрагент'
        },
        position: {
          __caption__: 'Должность'
        }
      }
    }
  }
});

export default translations;
