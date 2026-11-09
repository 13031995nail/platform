namespace Tmobis.Business.Common.Personnel;

using System.Linq;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;

public class AdmissionCommitteeEmployeeBS(
    IMessageDispatcher messageDispatcher,
    AdmissionCommitteeAccess access)
    : MessageBasedEntityBS<AdmissionCommitteeEmployee>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateAdmissionCommitteeEmployee(
        AdmissionCommitteeEmployee UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

    protected override async Task<List<Tmobis.Core.Models.IEntity>> OnEntityChangedAsync(
        AdmissionCommitteeEmployee entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}
