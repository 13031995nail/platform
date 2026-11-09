export default async function applyDocumentContext(store, model, params) {
  if (params.workObjectId) {
    const workObject = await store.findRecord('work-object', params.workObjectId,
      { projection: 'WorkObjectE' });
    model.set('workObject', workObject);
    model.set('organization', workObject.get('organization'));
  } else if (params.organizationId) {
    model.set('organization', await store.findRecord('organization', params.organizationId,
      { projection: 'OrganizationE' }));
  }
  if (params.awpId) {
    model.set('awp', await store.findRecord('awp', params.awpId, { projection: 'AwpE' }));
  }
}
