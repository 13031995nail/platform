import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  entityLoader: service('entity-loader'),

  getActType(id, projection = 'ActTypeL') {
    const modelName = 'act-type';
    return this.entityLoader.getById(id, modelName, projection);
  }
});
