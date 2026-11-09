namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;

public class RedinessActBS(
    IMessageDispatcher messageDispatcher,
    RedinessActAccess access)
: MessageBasedEntityBS<RedinessAct>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateRedinessAct(
        RedinessAct UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

    protected override async Task<List<Tmobis.Core.Models.IEntity>> OnEntityChangedAsync(
        RedinessAct entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
