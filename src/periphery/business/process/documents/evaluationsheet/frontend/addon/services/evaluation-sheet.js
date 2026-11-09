import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';

export class EvaluationSheetQueryBuilder {
  constructor(store) {
    this.store = store;
    this.organizationId = undefined;
    this.awpId = undefined;
    this.statusId = undefined;
    this.view = 'EvaluationSheetL';
  }

  setView(view) {
    this.view = view;
    return this;
  }

  filterOrganization(organizationId) {
    this.organizationId = organizationId;
    return this;
  }

  filterAwp(awpId) {
    this.awpId = awpId;
    return this;
  }

  filterStatus(statusId) {
    this.statusId = statusId;
    return this;
  }

  query() {
    const store = this.store;
    const modelName = 'evaluation-sheet';

    let predicates = [];

    if (this.organizationId) {
      predicates.push(new SimplePredicate(
        'organization', FilterOperator.Eq, this.organizationId));
    }

    if (this.awpId) {
      predicates.push(new SimplePredicate(
        'awp', FilterOperator.Eq, this.awpId));
    }

    if (this.statusId) {
      predicates.push(new SimplePredicate(
        'actStatus', FilterOperator.Eq, this.statusId));
    }

    let builder = new Builder(store, modelName)
      .selectByProjection(this.view);

    if (predicates.length > 0) {
      let predicate = (predicates.length > 1)
        ? new ComplexPredicate(Condition.And, ...predicates)
        : predicates[0];
      builder.where(predicate);
    }

    return store.query(modelName, builder.build());
  }
}

export default Service.extend({
  store: service(),

  getQueryBuilder() {
    return new EvaluationSheetQueryBuilder(this.store);
  }
});
