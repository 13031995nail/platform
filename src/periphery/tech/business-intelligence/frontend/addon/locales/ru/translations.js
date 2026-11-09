import $ from 'jquery';

import DashboardForm from './forms/dashboard';
import DashboardsForm from './forms/dashboards';
import DashboardModel from './models/dashboard';

const translations = {};
$.extend(true, translations, {
  models: {
    'dashboard': DashboardModel,
  },
  forms: {
    'dashboard': DashboardForm,
    'dashboards': DashboardsForm,
  }
});

export default translations;
