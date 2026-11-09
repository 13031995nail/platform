import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import AuthenticatedRouteMixin from 'ember-simple-auth/mixins/authenticated-route-mixin';

export default EditFormRoute.extend(AuthenticatedRouteMixin, {
  modelProjection: 'DashboardE',
  modelName: 'dashboard'
});
