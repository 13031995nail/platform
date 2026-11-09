import $ from 'jquery';
import TmobisWorkObjectLForm from './forms/work-object-l';
import TmobisWorkObjectEForm from './forms/work-object-e';
import TmobisWorkObjectModel from './models/work-object';

const translations = {};

$.extend(true, translations, {
    models: {
        'work-object': TmobisWorkObjectModel,
    },
    
    forms: {
        'work-object-l': TmobisWorkObjectLForm,
        'work-object-e': TmobisWorkObjectEForm,
    },
});

export default translations;
