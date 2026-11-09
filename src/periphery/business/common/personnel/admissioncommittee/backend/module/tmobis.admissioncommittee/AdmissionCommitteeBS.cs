namespace Tmobis.Business.Common.Personnel;

using System.Linq;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;

public class AdmissionCommitteeBS(
    IMessageDispatcher messageDispatcher,
    AdmissionCommitteeAccess access)
    : MessageBasedEntityBS<AdmissionCommittee>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateAdmissionCommittee(
        AdmissionCommittee UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

    protected override async Task<List<Tmobis.Core.Models.IEntity>> OnEntityChangedAsync(
        AdmissionCommittee entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
