import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';

export default EditFormNewRoute.extend({
  modelProjection: 'DashboardE',
  modelName: 'dashboard',
  templateName: 'tmobis-dashboard',
  controllerName: 'tmobis-dashboard'
});
