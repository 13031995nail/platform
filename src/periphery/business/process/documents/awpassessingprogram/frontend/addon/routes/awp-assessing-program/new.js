import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { inject as service } from '@ember/service';
import { translationMacro as t } from 'ember-i18n';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';
import { ActTypeProgramKey } from '../../models/awp-assessing-program';

export default EditFormNewRoute.extend({
  awpProvider: service(),
  actStatusProvider: service(),
  actTypeProvider: service(),

  modelProjection: 'AwpAssessingProgramE',
  modelName: 'awp-assessing-program',
  templateName: 'awp-assessing-program',

  masterTitle: t('forms.awp-assessing-program.caption'),

  async afterModel(model) {
    await this._super(...arguments);

    const actType = await this.actTypeProvider.getActType(ActTypeProgramKey);
    const actStatus = await this.actStatusProvider.getActStatus(ActStatusKeys.created);
    const awp = await this.awpProvider.getCurrentAwp();

    model.set('actType', actType);
    model.set('actStatus', actStatus);
    model.set('awp', awp);
    model.set('regDate', new Date());
  }
});
