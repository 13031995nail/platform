import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  store: service(),

  async findActualPlaceForEmployee(organization) {
    const store = this.store,
      modelName = 'place-of-employment',
      builder = new Builder(store, modelName)
        .selectByProjection('PlaceOfEmploymentD')
        .where(new ComplexPredicate(Condition.Or,
            new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
            new SimplePredicate('organization.hierarchy', FilterOperator.Eq, organization.get('id'))));

    return store.query(modelName, builder.build());
  },
});
