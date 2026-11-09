import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import { set } from '@ember/object';
import { translationMacro as t } from 'ember-i18n';
import AdmissionCommitteeStatusEnum from '../enums/admission-committee-status';

export default EditFormRoute.extend({
  modelProjection: 'AdmissionCommitteeE',
  modelName: 'admission-committee',
  
  masterTitle: t('forms.admission-committee.caption'),

  setupController(controller, model) {
    let status = model.get('status');
    this.set('readonly', !this.get('updateEnabled') || status === AdmissionCommitteeStatusEnum.Approved);
    this._super(...arguments);
    set(controller, 'leaderKey', model.get('leader.id'));
    set(controller, 'deputyLeaderKey', model.get('deputyLeader.id'));
    let store = this.store,
      admissions = model.get('admissionCommitteeEmployee');
    admissions.forEach(x => {
      let placeOfEmployment = store.createRecord('place-of-employment', {
        employee: x.employee
      });
      set(x, 'placeOfEmployment', placeOfEmployment);
    })
  },
});
