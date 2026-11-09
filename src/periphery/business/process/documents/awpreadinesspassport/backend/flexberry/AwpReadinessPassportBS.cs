namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;

public class AwpReadinessPassportBS(
    IMessageDispatcher messageDispatcher,
    AwpReadinessPassportAccess access)
: MessageBasedEntityBS<AwpReadinessPassport>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateAwpReadinessPassport(
        AwpReadinessPassport UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

    protected override async Task<List<Tmobis.Core.Models.IEntity>> OnEntityChangedAsync(
        AwpReadinessPassport entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
