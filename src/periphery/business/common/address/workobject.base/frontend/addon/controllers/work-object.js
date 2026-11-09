import DictionariesEditFormController from '@timepath/tmobis-core/controllers/base/edit-form-common';
import { inject as controller } from '@ember/controller';
import { computed } from '@ember/object';
import { SimplePredicate } from 'ember-flexberry-data/query/predicate';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';

export default DictionariesEditFormController.extend({
  applicationController: controller('application'),
  parentRoute: 'work-objects',

  objectFunctionCustomLimitPredicate: computed(function () {
    return new SimplePredicate('actuality', FilterOperator.Eq, true);
  }),
  organizationCustomLimitPredicate: computed(function () {
    return new SimplePredicate('actuality', FilterOperator.Eq, true);
  }),
});