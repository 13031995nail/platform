namespace Tmobis.Business.Common.Personnel;

using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnAdmissionCommitteeEmployeeDeleted(IRepository repository)
  : OnEntityUpdated<EntityDeleted<AdmissionCommitteeEmployee>, AdmissionCommitteeEmployee>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AdmissionCommitteeEmployee entity)
    {
        var toUpdate = new List<IEntity>();
        var admissioncommittees = await repository.Find<AdmissionCommittee>(x => x.Leader.Id == entity.Id ||
            x.DeputyLeader.Id == entity.Id);
        if (admissioncommittees.Count > 0)
        {
            foreach (var admissioncommittee in admissioncommittees)
            {
                admissioncommittee.Leader = null;
                admissioncommittee.DeputyLeader = null;
                toUpdate.AddRange(admissioncommittee);
            }
        }
        return toUpdate;
    }
}
