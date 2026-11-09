import $ from 'jquery';
import { buildValidations } from 'ember-cp-validations';

import {
  defineBaseModel,
  defineProjections,
  ValidationRules,
  Model as WorkTypeMixin
} from '../mixins/regenerated/models/work-type';

import DictionaryModel from '@timepath/tmobis-core/models/dictionary';
import { ValidationRules as ParentValidationRules } 
  from '@timepath/tmobis-core/models/actual-entity';

const Validations = buildValidations($.extend({}, 
  ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale'],
});

let Model = DictionaryModel.extend(WorkTypeMixin, Validations, {
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Resources.WorkType',
});

defineBaseModel(Model);
defineProjections(Model);

export default Model;
