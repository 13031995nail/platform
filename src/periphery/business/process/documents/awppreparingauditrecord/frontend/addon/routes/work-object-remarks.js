import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default Route.extend({
  processProvider: service('work-object-process'),
  rolesManager: service(),
  queryParams: { awpId: { refreshModel: true } },
  masterTitle: 'Замечания по объектам',

  model(params) {
    const activeWorkspace = this.get('rolesManager.activeWorkspace');
    const workspace = ['tso', 'administration'].includes(activeWorkspace) ? activeWorkspace : 'administration';
    return this.get('processProvider').remarks(workspace, params.awpId)
      .then(groups => ({ groups })).catch(() => ({ loadError: 'Не удалось загрузить замечания.' }));
  }
});
