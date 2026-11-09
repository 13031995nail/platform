namespace Tmobis.Business.Common.Personnel;

using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Repositories;

/// <summary>
/// Общие проверки и выборка членства сотрудника в утверждённых комиссиях.
/// </summary>
public sealed class AdmissionCommitteeMembershipProvider(
    IRepository repository,
    ICurrentRoleProvider roleProvider,
    ICurrentUserProvider currentUserProvider)
{
    public async Task<List<AdmissionCommitteeEmployee>> GetCurrentMemberships(
        Guid? awpId = null,
        Guid? expectedEmployeeId = null)
    {
        var roles = await roleProvider.GetCurrentRoleNames();
        var employeeId = (await currentUserProvider.GetCurrentEmployee())?.Id;
        if (!employeeId.HasValue
            || expectedEmployeeId.HasValue && employeeId.Value != expectedEmployeeId.Value
            || !roles.Contains(AdmissionCommitteeRoleNames.Member, StringComparer.OrdinalIgnoreCase))
        {
            return [];
        }

        return await GetApprovedMemberships(employeeId.Value, awpId);
    }

    public async Task<List<AdmissionCommitteeEmployee>> GetApprovedMemberships(
        Guid employeeId,
        Guid? awpId = null)
    {
        if (employeeId == Guid.Empty)
        {
            return [];
        }

        return await repository.Find<AdmissionCommitteeEmployee>(member =>
            member.Employee.Id == employeeId
            && (!awpId.HasValue || member.AdmissionCommittee.Awp.Id == awpId.Value)
            && member.AdmissionCommittee.Status == AdmissionCommitteeStatus.Approved,
            view: AdmissionCommitteeEmployee.Views.AdmissionCommitteeEmployeeReviewEntityView);
    }
}
