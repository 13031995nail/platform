import DictionariesEditFormController from '@timepath/tmobis-core/controllers/base/dictionaries-edit-form-common';
import { computed } from '@ember/object';
import { SimplePredicate } from 'ember-flexberry-data/query/predicate';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';

export default DictionariesEditFormController.extend({
  parentRoute: 'verification-requierements',

  actualityCustomLimitPredicate: computed(function () {
    return new SimplePredicate('actuality', FilterOperator.Eq, true);
  })
});