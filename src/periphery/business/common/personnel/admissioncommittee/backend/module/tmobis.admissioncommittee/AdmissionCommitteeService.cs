namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;
using Core.Flexberry.Models;
using Core.Providers.Roles;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;

public class AdmissionCommitteeService(
   IRepository repository,
   IRoleProvider roleProvider)
{

    public bool ShouldGrantCommissionMemberRole(AdmissionCommittee entity)
 => entity.Status == AdmissionCommitteeStatus.Approved;

    public async Task GrantCommissionMemberRole(AdmissionCommittee entity)
    {
        var memberIds = await GetMemberEmployeeIds(entity);
        if (memberIds.Count == 0)
        {
            return;
        }

        var employees = await repository.GetByIds<Employee>(
            memberIds,
            new EntityView(nameof(Employee.Views.EmployeeE)));
        foreach (var user in employees
            .Where(x => x.User != null)
            .Select(x => x.User!)
            .GroupBy(x => x.Id)
            .Select(x => x.First()))
        {
            if (await roleProvider.HasRole(user, AdmissionCommitteeRoleNames.Member))
            {
                continue;
            }

            await roleProvider.GrantRole(user, AdmissionCommitteeRoleNames.Member);
        }
    }

    public async Task<List<Guid>> GetMemberEmployeeIds(AdmissionCommittee entity)
    {
        var memberIds = new List<Guid>();
        if (entity.CheckLoadedProperty(nameof(AdmissionCommittee.AdmissionCommitteeEmployee)))
        {
            memberIds.AddRange(entity.AdmissionCommitteeEmployee
                .Cast<AdmissionCommitteeEmployee>()
                .Where(x => ((DataObject)x).GetStatus() != ObjectStatus.Deleted)
                .Select(x => x.Employee.Id));
        }
        else
        {
            var members = await repository.Find<AdmissionCommitteeEmployee>(
                x => x.AdmissionCommittee.Id == entity.Id);
            memberIds.AddRange(members.Select(x => x.Employee.Id));
        }

        return [.. memberIds
            .Where(x => x != Guid.Empty)
            .Distinct()];
    }
}
