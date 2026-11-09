import $ from 'jquery';
import TmobisPlaceOfEmploymentForm from './forms/place-of-employment';

const translations = {};

$.extend(true, translations, {
    forms: {
        'place-of-employment': TmobisPlaceOfEmploymentForm,
    },
});

export default translations;