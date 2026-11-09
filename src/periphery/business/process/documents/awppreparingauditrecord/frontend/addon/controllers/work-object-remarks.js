import Controller from '@ember/controller';
import { computed } from '@ember/object';

export default Controller.extend({
  queryParams: ['awpId'],
  awpId: null,
  onlyOpen: true,

  groups: computed('model.groups.[]', 'onlyOpen', function () {
    return (this.get('model.groups') || []).map(group => Object.assign({}, group, {
      requirements: group.requirements.filter(item => !this.get('onlyOpen') || !item.isCompleted)
    })).filter(group => group.requirements.length > 0);
  })
});
