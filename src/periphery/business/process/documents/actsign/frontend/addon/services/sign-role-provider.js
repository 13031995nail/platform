import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  entityLoader: service('entity-loader'),

  getSignRole(id, projection = 'SignRoleL') {
    const modelName = 'sign-role';
    return this.entityLoader.getById(id, modelName, projection);
  }
});
