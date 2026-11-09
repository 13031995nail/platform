import $ from 'jquery';
import TmobisObjectFunctionLForm from './forms/object-function-l';
import TmobisObjectFunctionEForm from './forms/object-function-e';
import TmobisObjectFunctionModel from './models/object-function';

const translations = {};

$.extend(true, translations, {
    models: {
        'object-function': TmobisObjectFunctionModel,
    },
    
    forms: {
        'object-function-l': TmobisObjectFunctionLForm,
        'object-function-e': TmobisObjectFunctionEForm,
    },
});

export default translations;