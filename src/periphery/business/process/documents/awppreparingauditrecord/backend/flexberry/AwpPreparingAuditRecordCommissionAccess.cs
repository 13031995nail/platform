namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;

/// <summary>
/// Проверка полномочий председателя комиссии для журнала указанного ОЗП.
/// </summary>
public sealed class AwpPreparingAuditRecordCommissionAccess(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    ICurrentUserProvider currentUserProvider,
    AdmissionCommitteeMembershipProvider membershipProvider)
{
    public async Task<List<AdmissionCommittee>> GetCommittees(Guid awpId, Guid employeeId)
    {
        if (awpId == Guid.Empty || employeeId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Не заданы ОЗП и председатель комиссии.");
        }

        List<AdmissionCommitteeEmployee> memberships;
        if (await accessProvider.IsTechnicalAdministrator())
        {
            memberships = await membershipProvider.GetApprovedMemberships(employeeId, awpId);
        }
        else
        {
            var employee = await currentUserProvider.GetCurrentEmployee();
            if (employee?.Id != employeeId)
            {
                throw new UnauthorizedAccessException("Операция доступна только текущему председателю комиссии.");
            }

            memberships = await membershipProvider.GetCurrentMemberships(awpId, employeeId);
        }

        var committees = await repository.Find<AdmissionCommittee>(committee =>
            committee.Awp.Id == awpId
            && committee.Status == AdmissionCommitteeStatus.Approved
            && (committee.Leader != null && committee.Leader.Employee.Id == employeeId
                || committee.DeputyLeader != null && committee.DeputyLeader.Employee.Id == employeeId),
            view: new EntityView("AdmissionCommitteeE"));
        var committeeIds = memberships.Select(member => member.AdmissionCommittee.Id).ToHashSet();
        committees = committees.Where(committee => committeeIds.Contains(committee.Id)).ToList();
        if (committees.Count == 0)
        {
            throw new UnauthorizedAccessException("Не найдена действующая комиссия председателя для ОЗП.");
        }

        return committees;
    }

    public async Task<(AdmissionCommittee Committee, WorkObject WorkObject)> GetCommitteeForObject(
        Guid awpId, Guid employeeId, Guid workObjectId)
    {
        var committees = await GetCommittees(awpId, employeeId);
        var workObject = await repository.GetById<WorkObject>(workObjectId,
            WorkObject.Views.WorkObjectEEntityView);
        var committee = committees.FirstOrDefault(item =>
            workObject?.Organization != null
            && (item.Organization.Id == workObject.Organization.Id
                || item.Organization.Id == workObject.Organization.Hierarchy?.Id));
        if (committee == null || workObject == null || !workObject.Actuality)
        {
            throw new UnauthorizedAccessException("Объект не относится к действующей комиссии.");
        }

        return (committee, workObject);
    }
}
