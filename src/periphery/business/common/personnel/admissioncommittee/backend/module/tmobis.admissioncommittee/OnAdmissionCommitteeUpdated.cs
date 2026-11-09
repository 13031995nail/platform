namespace Tmobis.Business.Common.Personnel;

using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public class OnAdmissionCommitteeUpdated(
   AdmissionCommitteeService service)
: OnEntityUpdated<EntityUpdated<AdmissionCommittee>, AdmissionCommittee>(null)
{
    private readonly AdmissionCommitteeService service = service;

    protected override async ValueTask<List<IEntity>> ChangeEntity(AdmissionCommittee entity)
    {
        return [];
    }

    protected override async ValueTask FinishEntity(AdmissionCommittee entity)
    {
        if (service.ShouldGrantCommissionMemberRole(entity))
        {
            await service.GrantCommissionMemberRole(entity);
        }
    }
}
