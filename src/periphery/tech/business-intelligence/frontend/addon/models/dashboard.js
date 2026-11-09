import {
  Model as DashboardMixin,
  defineProjections,
  defineBaseModel
} from
  '../mixins/regenerated/models/dashboard';
import BaseModel from '@timepath/tmobis-core/models/base-model';
import { buildValidations } from 'ember-cp-validations';

const Validations = buildValidations({}, {
  dependentKeys: ['model.i18n.locale'],
});

let Model = BaseModel.extend(DashboardMixin, Validations, {

});

Model.reopenClass({
  namespace: 'Tmobis.BusinessIntelligence',
});

defineProjections(Model);
defineBaseModel(Model);

export default Model;
