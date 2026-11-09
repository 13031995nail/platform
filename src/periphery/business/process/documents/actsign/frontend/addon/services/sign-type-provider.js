import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  entityLoader: service('entity-loader'),

  getSignType(id, projection = 'SignTypeL') {
    const modelName = 'sign-type';
    return this.entityLoader.getById(id, modelName, projection);
  }
});
