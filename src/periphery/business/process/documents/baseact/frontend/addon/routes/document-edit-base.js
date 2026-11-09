import EditFormRoute from '@timepath/tmobis-core/routes/base/edit-form';
import ActStatusKeys from '@timepath/tmobis-business-process-acts-base-act/enums/act-status';

/**
 * Форма редактирования документа
 */
export default EditFormRoute.extend({
  setupController(controller, model) {
    let status = model.get('actStatus.id');
    this.set('readonly', !this.get('updateEnabled') || !(status === ActStatusKeys.editing || status === ActStatusKeys.created));
    this._super(...arguments);
    this.set('masterNumber', model.get('actNumber'));
  },
});
