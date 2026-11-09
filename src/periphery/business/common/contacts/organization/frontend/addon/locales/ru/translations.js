import $ from 'jquery';
import TmobisOrganizationLForm from './forms/organization-l';
import TmobisOrganizationEForm from './forms/organization-e';
import TmobisOrganizationModel from './models/organization';

const translations = {};

$.extend(true, translations, {
    models: {
        'organization': TmobisOrganizationModel,
    },
    
    forms: {
        'organization-l': TmobisOrganizationLForm,
        'organization-e': TmobisOrganizationEForm,
    },
});

export default translations;
