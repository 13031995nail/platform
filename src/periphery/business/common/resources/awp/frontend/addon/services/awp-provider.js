import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  store: service(),

  getCurrentAwp() {
    const store = this.store;
    const modelName = 'awp';
    const builder = new Builder(store, modelName)
    .selectByProjection('AwpL')
    .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('actuality', FilterOperator.Eq, true),
        new SimplePredicate('isCurrent', FilterOperator.Eq, true)))
    .top(1);

    return store.queryRecord(modelName, builder.build());
  }
});
