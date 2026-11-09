import EditFormController from '@timepath/tmobis-core/controllers/base/edit-form-common';
import { getOwner } from '@ember/application';

export default EditFormController.extend({
  parentRoute: 'awp-preparing-audit-records',
  hasSaveButton: false,

  actions: {
    viewEvaluationSheet() {
      this.transitionToRoute('evaluation-sheet', this.get('model.evaluationSheet.id'), {
        queryParams: {
          backRoute: getOwner(this).lookup('controller:application').currentRouteName,
          backId: this.get('model.id')
        }
      });
    },
    viewReadinessAct() {
      this.transitionToRoute('rediness-act', this.get('model.readinessAct.id'), {
        queryParams: {
          backRoute: getOwner(this).lookup('controller:application').currentRouteName,
          backId: this.get('model.id')
        }
      });
    },
    viewAwpReadinessPassport() {
      this.transitionToRoute('awp-readiness-passport', this.get('model.awpReadinessPassport.id'), {
        queryParams: {
          backRoute: getOwner(this).lookup('controller:application').currentRouteName,
          backId: this.get('model.id')
        }
      });
    },
    viewWorkObject() {
      this.transitionToRoute('work-object', this.get('model.workObject.id'), {
        queryParams: {
          backRoute: getOwner(this).lookup('controller:application').currentRouteName,
          backId: this.get('model.id')
        }
      });
    },
  }
});
