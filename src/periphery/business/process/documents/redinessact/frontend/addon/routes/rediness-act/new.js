import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';
import { inject as service } from '@ember/service';
import { translationMacro as t } from 'ember-i18n';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';
import { ActTypeReadinessActKey } from '../../models/rediness-act';
import applyDocumentContext from '@timepath/tmobis-business-process-acts-base-act/utils/document-context';

export default EditFormNewRoute.extend({
  awpProvider: service(),
  actStatusProvider: service(),
  actTypeProvider: service(),

  modelProjection: 'RedinessActE',
  modelName: 'rediness-act',
  templateName: 'rediness-act',
  
  masterTitle: t('forms.rediness-act.caption'),

  async afterModel(model, transition) {
    await this._super(...arguments);

    const actType = await this.actTypeProvider.getActType(ActTypeReadinessActKey);
    const actStatus = await this.actStatusProvider.getActStatus(ActStatusKeys.created);
    const awp = await this.awpProvider.getCurrentAwp();
    
    model.set('actType', actType);
    model.set('actStatus', actStatus);
    model.set('awp', awp);
    model.set('regDate', new Date());
    await applyDocumentContext(this.store, model, transition.queryParams || {});
  }
});
