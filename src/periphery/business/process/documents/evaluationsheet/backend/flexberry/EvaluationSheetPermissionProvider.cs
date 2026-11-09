namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Core.Repositories;

/// <summary>
/// Проверка полномочий подготовки и проверки для объекта и ОЗП.
/// </summary>
public sealed class EvaluationSheetPermissionProvider(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    ICurrentRoleProvider roleProvider,
    AdmissionCommitteeMembershipProvider membershipProvider)
{
    private readonly Dictionary<Guid, WorkObject?> workObjects = [];
    private readonly Dictionary<Guid, WorkObjectRoleAccess> roleAccess = [];

    public async Task<EvaluationSheetPermissions> GetPermissions(
        Guid? workObjectId = null, Guid? awpId = null, WorkObject? workObject = null)
    {
        if (await accessProvider.IsTechnicalAdministrator())
        {
            return new(true, true, true);
        }
        var roles = await roleProvider.GetCurrentRoleNames();
        if (!workObjectId.HasValue)
        {
            var canFormAct = roles.Contains(WorkObjectRoleNames.Tso, StringComparer.OrdinalIgnoreCase)
                || roles.Contains(WorkObjectRoleNames.Administration, StringComparer.OrdinalIgnoreCase);
            return new(roles.Contains(WorkObjectRoleNames.Consumer, StringComparer.OrdinalIgnoreCase),
                roles.Any(role => role.Equals(WorkObjectRoleNames.Tso, StringComparison.OrdinalIgnoreCase)
                    || role.Equals(WorkObjectRoleNames.Administration, StringComparison.OrdinalIgnoreCase)
                    || role.Equals(AdmissionCommitteeRoleNames.Member, StringComparison.OrdinalIgnoreCase)), canFormAct);
        }
        if (workObject == null && !workObjects.TryGetValue(workObjectId.Value, out workObject))
        {
            workObject = await repository.GetById<WorkObject>(workObjectId.Value,
                WorkObject.Views.WorkObjectEEntityView);
            workObjects[workObjectId.Value] = workObject;
        }
        if (workObject?.Organization == null || workObject.Id != workObjectId.Value)
        {
            return new(false, false, false);
        }
        workObjects[workObjectId.Value] = workObject;
        if (!roleAccess.TryGetValue(workObjectId.Value, out var access))
        {
            access = await accessProvider.GetRoleAccess(workObject);
            roleAccess[workObjectId.Value] = access;
        }
        var canReview = access.Tso || access.Administration;
        if (!canReview && awpId.HasValue
            && roles.Contains(AdmissionCommitteeRoleNames.Member, StringComparer.OrdinalIgnoreCase))
        {
            var employeeId = await accessProvider.GetCurrentEmployeeId();
            var organizationId = workObject.Organization.Id;
            var hierarchyId = workObject.Organization.Hierarchy?.Id;
            if (employeeId.HasValue)
            {
                var memberships = await membershipProvider.GetApprovedMemberships(
                    employeeId.Value, awpId.Value);
                canReview = memberships.Any(member =>
                    member.AdmissionCommittee.Organization.Id == organizationId
                    || hierarchyId.HasValue
                        && member.AdmissionCommittee.Organization.Id == hierarchyId.Value);
            }
        }
        return new(access.Consumer, canReview, access.Tso || access.Administration);
    }

    public async Task<List<WorkObject>> GetCommissionObjects(Guid awpId)
    {
        var members = await GetCommissionMemberships(awpId);
        var organizationIds = members.Select(x => x.AdmissionCommittee.Organization.Id)
            .Distinct().Cast<object>().ToList();
        var result = new List<WorkObject>();
        foreach (var batch in organizationIds.Chunk(500))
        {
            var ids = batch.ToList();
            result.AddRange(await repository.Find<WorkObject>(workObject => workObject.Actuality
                && (ids.Contains(workObject.Organization.Id) || ids.Contains(workObject.Organization.Hierarchy.Id)),
                view: WorkObject.Views.WorkObjectEEntityView));
        }
        return result.DistinctBy(x => x.Id).ToList();
    }

    public Task<List<AdmissionCommitteeEmployee>> GetCommissionMemberships(Guid? awpId = null)
        => membershipProvider.GetCurrentMemberships(awpId);
}

public sealed record EvaluationSheetPermissions(bool CanSubmit, bool CanReview, bool CanFormAct);
