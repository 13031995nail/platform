namespace Tmobis.Business.Common.Personnel;

using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public class OnAdmissionCommitteeDeleted()
: OnEntityUpdated<EntityDeleted<AdmissionCommittee>, AdmissionCommittee>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AdmissionCommittee entity)
    {
        entity.Leader = null;
        entity.DeputyLeader = null;
        return [];
    }
}
