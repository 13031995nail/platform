import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';
import AdmissionCommitteeStatusEnum from '../enums/admission-committee-status';
import { getOwner } from '@ember/application';

export default Service.extend({
  store: service(),

  getCommittee(organization, awp) {
    const store = this.store;
    const modelName = 'admission-committee';
    const builder = new Builder(store, modelName)
    .selectByProjection('AdmissionCommitteeE')
    .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
        new SimplePredicate('awp', FilterOperator.Eq, awp.get('id'))))
    .top(1);

    return store.queryRecord(modelName, builder.build());
  },
  getApprovedCommitteeByLead(leaderId, awpId) {
    const store = this.store;
    const modelName = 'admission-committee';
    const statusApproved = getOwner(this).lookup('transform:admission-committee-status').serialize(AdmissionCommitteeStatusEnum.Approved);
    const builder = new Builder(store, modelName)
      .selectByProjection('AdmissionCommitteeE')
      .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('leader.employee', FilterOperator.Eq, leaderId),
        new SimplePredicate('awp', FilterOperator.Eq, awpId),
        new SimplePredicate('status', FilterOperator.Eq, statusApproved)))
      .top(1);

    return store.queryRecord(modelName, builder.build());
  },
  getApprovedCommittee(organization, awp) {
    const store = this.store;
    const modelName = 'admission-committee';
    const statusApproved = getOwner(this).lookup('transform:admission-committee-status').serialize(AdmissionCommitteeStatusEnum.Approved);
    const builder = new Builder(store, modelName)
      .selectByProjection('AdmissionCommitteeE')
      .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('organization', FilterOperator.Eq, organization.get('hierarchy') ? organization.get('hierarchy.id') : organization.get('id')),
        new SimplePredicate('awp', FilterOperator.Eq, awp.get('id')),
        new SimplePredicate('status', FilterOperator.Eq, statusApproved)))
      .top(1);

    return store.queryRecord(modelName, builder.build());
  }
});
