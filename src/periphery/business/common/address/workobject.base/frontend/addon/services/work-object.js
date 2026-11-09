import Builder from 'ember-flexberry-data/query/builder';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  store: service(),

  /**
   * Базовый билдер запроса объектов у организации.
   * @param {string} organizationId ID организации для поиска объектов.
   * @private
   */
  getObjectsByOrganizationBuilder(organizationId) {
    const store = this.store;
    const modelName = 'work-object';

    const builder = new Builder(store, modelName)
      .selectByProjection('WorkObjectL')
      .where(new SimplePredicate(
        'organization', FilterOperator.Eq, organizationId));

    return builder;
  },

  /**
   * Запрос количества объектов у организации.
   * @param {string} organizationId ID организации для поиска объектов.
   */
  async countObjectByOrganization(organizationId) {
    const store = this.store;
    const modelName = 'work-object';

    const query = this
    .getObjectsByOrganizationBuilder(organizationId)
    .count()
    .build();

    return (await store.query(modelName, query)).get('length');
  }
});
