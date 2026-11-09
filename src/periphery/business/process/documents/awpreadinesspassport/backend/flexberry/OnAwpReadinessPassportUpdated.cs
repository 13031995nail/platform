namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Models;

public class OnAwpReadinessPassportUpdated(IMessageDispatcher messageDispatcher)
: OnEntityUpdated<EntityUpdated<AwpReadinessPassport>, AwpReadinessPassport>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AwpReadinessPassport entity)
    {
        var toUpdate = new List<IEntity>();
        var changesAttrs = entity.GetAlteredPropertyNames();
        var status = Information.ExtractPropertyName<AwpReadinessPassport>(f => f.ActStatus);
        if (changesAttrs.Contains(status) && entity.ActStatus.Id == IActStatus.Keys.Signed)
        {
            AwpReadinessPassportSigned msg = new AwpReadinessPassportSigned() { Act = entity };
            var result = await messageDispatcher.SendWithResult<AwpReadinessPassportSigned, IEntityUpdatedResult>(msg);
            if (result != null)
            {
                toUpdate.AddRange(result.EntitiesToUpdate);
            }
        }
        return toUpdate;
    }
}
