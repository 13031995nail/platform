namespace Tmobis.Business.Common.Personnel;

using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public class OnAdmissionCommitteeEmployeeCreated(
   AdmissionCommitteeEmployeeService service)
: OnEntityUpdated<EntityCreated<AdmissionCommitteeEmployee>, AdmissionCommitteeEmployee>(null)
{
    private readonly AdmissionCommitteeEmployeeService service = service;


    protected override async ValueTask<List<IEntity>> ChangeEntity(AdmissionCommitteeEmployee entity)
    {
        await service.UpdatePlaces(entity);
        return [];
    }
}
