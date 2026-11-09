import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';

export default Service.extend({
  store: service(),

  async findAct(organization, awp) {
    const store = this.store;
    const modelName = 'awp-assessing-program';
    const builder = new Builder(store, modelName)
      .selectByProjection('AwpAssessingProgramL')
    .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
        new SimplePredicate('awp', FilterOperator.Eq, awp.get('id'))))
    .top(1);

    return store.queryRecord(modelName, builder.build());
  },
  async findApprovedAct(organization, awp) {
    const store = this.store;
    const modelName = 'awp-assessing-program';
    const builder = new Builder(store, modelName)
      .selectByProjection('AwpAssessingProgramL')
      .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
        new SimplePredicate('awp', FilterOperator.Eq, awp.get('id')),
        new SimplePredicate('actStatus', FilterOperator.Eq, ActStatusKeys.approved)))
      .top(1);

    return store.queryRecord(modelName, builder.build());
  }
});
