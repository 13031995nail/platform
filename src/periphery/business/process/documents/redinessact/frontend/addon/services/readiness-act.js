import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';
import { inject as service, default as Service } from '@ember/service';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';
import PartnerReadinessLevel from '../enums/partner-readiness-level';
import { getOwner } from '@ember/application';
import Arrays from '@timepath/tmobis-core/utils/arrays';

export default Service.extend({
  store: service(),
  workObjectService: service('work-object'),
  evaluationSheetService: service('evaluation-sheet'),

  async findAct(organization, awp) {
    const store = this.store;
    const modelName = 'rediness-act';
    const builder = new Builder(store, modelName)
    .selectByProjection('RedinessActE')
    .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
        new SimplePredicate('awp', FilterOperator.Eq, awp.get('id')),
        new SimplePredicate('actStatus', FilterOperator.Eq, ActStatusKeys.signed)))
    .orderBy('changeStatusDate desc')
    .top(1);

    return store.queryRecord(modelName, builder.build());
  },

  async findExistingAct(organization, awp) {
    const store = this.store;
    const modelName = 'rediness-act';
    const partnerReadinessNotReady = getOwner(this).lookup('transform:partner-readiness-level').serialize(PartnerReadinessLevel.NotReady);
    const builder = new Builder(store, modelName)
      .selectByProjection('RedinessActE')
      .where(new ComplexPredicate(
        Condition.And,
        new SimplePredicate('organization', FilterOperator.Eq, organization.get('id')),
        new SimplePredicate('awp', FilterOperator.Eq, awp.get('id')),
        new ComplexPredicate(Condition.Or, new SimplePredicate('actStatus', FilterOperator.Neq, ActStatusKeys.signed),
          new ComplexPredicate(Condition.And, new SimplePredicate('actStatus', FilterOperator.Eq, ActStatusKeys.signed),
            new SimplePredicate('partnerReadiness', FilterOperator.Neq, partnerReadinessNotReady))
        )))
      .orderBy('changeStatusDate desc')
      .top(1);

    return store.queryRecord(modelName, builder.build());
  },

  /**
   * Проверка при создании акта ООГ:
   * Для всех объектов лица в текущем ОЗП существует подписанный оценочный лист.
   * @see {@link https://gitlab.timepath.ru/timepath/platform/tmobis/products/ddtc/-/issues/123|постановка}
   * @param {string} organizationId ID организации для фильтрации объектов
   * @param {string} awpId ID ОЗП для фильтрации объектов
   */
  async checkObjectsEvaluationSheets(organizationId, awpId) {
    const objectCount = await this.workObjectService
      .countObjectByOrganization(organizationId);
    const sheets = await this.evaluationSheetService
      .getQueryBuilder()
      .filterOrganization(organizationId)
      .filterAwp(awpId)
      .filterStatus(ActStatusKeys.signed)
      .query();
    // по одному объекту может быть несколько ОЛ в рамках одного ОЗП
    return objectCount === Arrays.unique(sheets.mapBy('workObject.id')).get('length');
  }
});
