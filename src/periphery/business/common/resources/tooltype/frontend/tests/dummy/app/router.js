import EmberRouter from '@ember/routing/router';
import config from './config/environment';

const Router = EmberRouter.extend({
  location: config.locationType
});

Router.map(function () {
  this.route('towns');
  this.route('town',
  { path: 'town/:id' });
  this.route('town.new',
  { path: 'town/new' });
});

export default Router;
