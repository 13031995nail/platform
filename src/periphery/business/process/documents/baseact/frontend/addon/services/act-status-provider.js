import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  entityLoader: service('entity-loader'),

  getActStatus(id, projection = 'ActStatusL') {
    const modelName = 'act-status';
    return this.entityLoader.getById(id, modelName, projection);
  }
});
