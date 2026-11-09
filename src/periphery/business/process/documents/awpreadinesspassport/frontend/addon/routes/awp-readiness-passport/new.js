import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { inject as service } from '@ember/service';
import { translationMacro as t } from 'ember-i18n';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';
import { ActTypeReadinessPassportKey } from '../../models/awp-readiness-passport';
import applyDocumentContext from '@timepath/tmobis-business-process-acts-base-act/utils/document-context';

export default EditFormNewRoute.extend({
  awpProvider: service(),
  actStatusProvider: service(),
  actTypeProvider: service(),
  employeeProvider: service(),

  modelProjection: 'AwpReadinessPassportE',
  modelName: 'awp-readiness-passport',
  templateName: 'awp-readiness-passport',

  masterTitle: t('forms.awp-readiness-passport.caption'),

  async afterModel(model, transition) {
    await this._super(...arguments);

    const actType = await this.actTypeProvider.getActType(ActTypeReadinessPassportKey);
    const actStatus = await this.actStatusProvider.getActStatus(ActStatusKeys.created);
    const awp = await this.awpProvider.getCurrentAwp();
    const currentEmployee = await this.employeeProvider.getCurrentEmployee();

    model.set('actType', actType);
    model.set('actStatus', actStatus);
    model.set('awp', awp);
    model.set('regDate', new Date());
    model.set('assignee', currentEmployee);
    await applyDocumentContext(this.store, model, transition.queryParams || {});
  }
});
