import DictionariesEditFormController from '@timepath/tmobis-core/controllers/base/edit-form-common';

import { computed } from '@ember/object';
import { isEmpty } from '@ember/utils';
import { Promise } from 'rsvp';

import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import QueryBuilder from 'ember-flexberry-data/query/builder';
import { ComplexPredicate, SimplePredicate } from 'ember-flexberry-data/query/predicate';

import OrganizationTypeEnum from '@timepath/tmobis-business-common-contacts-organizationtype/enums/keys/organization-type';

export default DictionariesEditFormController.extend({
  parentRoute: 'organizations',
  
  hierarchyCustomLimitPredicate: computed(function () {
    return new ComplexPredicate(Condition.And, ...[
      new SimplePredicate('organizationType', FilterOperator.Eq, OrganizationTypeEnum.Municipality),
      new SimplePredicate('actuality', FilterOperator.Eq, true)
    ]);
  }),
  organizationTypeCustomLimitPredicate: computed(function () {
    return new SimplePredicate('actuality', FilterOperator.Eq, true);
  }),

  customPresave() {
    if (!isEmpty(this.model.acronym)) {
      let builder = new QueryBuilder(this.store)
        .from('organization')
        .selectByProjection('OrganizationE')
        .where(new ComplexPredicate(Condition.And, ...[
            new SimplePredicate('actuality', FilterOperator.Eq, true),
            new SimplePredicate('acronym', FilterOperator.Eq, this.model.acronym),
            new SimplePredicate('id', FilterOperator.Neq, this.model.id)]))
        .top(1);
      return this.store.query('organization', builder.build()).then(data => {
        if (data.get('length') > 0) {
          return Promise.resolve(this.createSaveResult(false, 'Контрагент с указанным актронимом уже существует'));
        } else {
          return Promise.resolve(this.createSaveResult(true));
        }
      })
    } else {
      return Promise.resolve(this.createSaveResult(true));
    }
  },
});
