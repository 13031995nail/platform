import $ from 'jquery';

import OrganizationTypeLForm from './forms/organization-types';
import OrganizationTypeEForm from './forms/organization-type';
import OrganizationTypeModel from './models/organization-type';

const translations = {};
$.extend(true, translations, {
  models: {
    'organization-type': OrganizationTypeModel,
  },
  forms: {
    'organization-types': OrganizationTypeLForm,
    'organization-type': OrganizationTypeEForm,
  }
});

export default translations;
