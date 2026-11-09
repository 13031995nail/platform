import $ from 'jquery';
import SignTypesForm from './forms/sign-types';
import SignTypeForm from './forms/sign-type';
import SignTypeModel from './models/sign-type';

import SignRoleModel from './models/sign-role';
import SignRolesForm from './forms/sign-roles';
import SignRoleForm from './forms/sign-role';

import ActSignModel from './models/act-sign';

const translations = {};

$.extend(true, translations, {
  models: {
    'act-sign': ActSignModel,
    'sign-type': SignTypeModel,
    'sign-role': SignRoleModel,
  },
  forms: {
    'sign-types': SignTypesForm,
    'sign-type': SignTypeForm,
    'sign-roles': SignRolesForm,
    'sign-role': SignRoleForm,
  }
});

export default translations;
