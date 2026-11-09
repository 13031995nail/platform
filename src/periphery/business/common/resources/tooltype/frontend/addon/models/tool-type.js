import $ from 'jquery';
import { buildValidations } from 'ember-cp-validations';

import {
  defineBaseModel,
  defineProjections,
  ValidationRules,
  Model as ToolTypeMixin
} from '../mixins/regenerated/models/tool-type';

import DictionaryModel from '@timepath/tmobis-core/models/dictionary';
import { ValidationRules as ParentValidationRules } 
  from '@timepath/tmobis-core/models/actual-entity';

const Validations = buildValidations($.extend({}, 
  ParentValidationRules, ValidationRules), {
  dependentKeys: ['model.i18n.locale'],
});

let Model = DictionaryModel.extend(ToolTypeMixin, Validations, {
});

Model.reopenClass({
  namespace: 'Tmobis.Business.Common.Resources.ToolType',
});

defineBaseModel(Model);
defineProjections(Model);

export default Model;
