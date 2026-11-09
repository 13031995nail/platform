namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Models;

public class OnRedinessActUpdated(IMessageDispatcher messageDispatcher)
: OnEntityUpdated<EntityUpdated<RedinessAct>, RedinessAct>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(RedinessAct entity)
    {
        var toUpdate = new List<IEntity>();
        var changesAttrs = entity.GetAlteredPropertyNames();
        var status = Information.ExtractPropertyName<RedinessAct>(f => f.ActStatus);
        if (changesAttrs.Contains(status) && entity.ActStatus.Id == IActStatus.Keys.Signed)
        {
            ReadinessActSigned msg = new ReadinessActSigned() { Act = entity };
            var result = await messageDispatcher.SendWithResult<ReadinessActSigned, IEntityUpdatedResult>(msg);
            if (result != null)
            {
                toUpdate.AddRange(result.EntitiesToUpdate);
            }
        }
        return toUpdate;
    }
}
