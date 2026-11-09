import Route from '@timepath/tmobis-business-process-signs-stilus/routes/employee';
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
        .from('place-of-employment')
        .selectByProjection('PlaceOfEmploymentD')
        .where(new SimplePredicate('employee', FilterOperator.Eq, model.id));
    promises.push(this.store.query('place-of-employment', builder.build()).then(data =>
      self.set('placeOfEmployments', data))
    );
    return Promise.all(promises);
  },

  setupController: function(controller) {
    this._super(...arguments);
    set(controller, 'placeOfEmployments', this.placeOfEmployments);
  }
});