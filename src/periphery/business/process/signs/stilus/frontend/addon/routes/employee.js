import Route from '@timepath/tmobis-core/routes/employee';
import QueryBuilder from 'ember-flexberry-data/query/builder';
import { SimplePredicate } from 'ember-flexberry-data/query/predicate';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { set } from '@ember/object';

export default Route.extend({
  afterModel: function(model) {
    this._super(...arguments);
    let self = this,
      promises = [],
      builder = new QueryBuilder(this.store)
        .from('employee-signature')
        .selectByProjection('EmployeeSignatureE')
        .where(new SimplePredicate('employee', FilterOperator.Eq, model.id));
    promises.push(this.store.query('employee-signature', builder.build()).then(data =>
      self.set('employeeSignature', data.firstObject))
    );
    return Promise.all(promises);
  },

  setupController: function(controller) {
    this._super(...arguments);
    set(controller, 'employeeSignature', this.employeeSignature);
  }
});