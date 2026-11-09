import Builder from 'ember-flexberry-data/query/builder';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';

export default Service.extend({
  store: service(),

  async findSignature(employee) {
    const store = this.store;
    const modelName = 'employee-signature';
    const builder = new Builder(store, modelName)
      .selectByProjection('EmployeeSignatureE')
    .where(new SimplePredicate('employee', FilterOperator.Eq, employee.get('id')))
    .top(1);

    return store.queryRecord(modelName, builder.build());
  }
});
