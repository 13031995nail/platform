namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

/// <summary>
/// Ограничивает управление комиссиями административной ролью и её территорией.
/// </summary>
public sealed class AdmissionCommitteeAccess(
    ICurrentRoleProvider roleProvider,
    ICurrentTownProvider townProvider,
    IRepository repository)
{
    public async Task EnsureCanChange(AdmissionCommittee committee)
    {
        var roles = new HashSet<string>(
            await roleProvider.GetCurrentRoleNames(),
            StringComparer.OrdinalIgnoreCase);
        if (roles.Contains(BusinessRoleNames.Admin)
            || roles.Contains(BusinessRoleNames.Administrator))
        {
            return;
        }

        if (!roles.Contains(BusinessRoleNames.Administration))
        {
            throw new UnauthorizedAccessException(
                "Управление комиссией доступно только Администрации.");
        }

        var organizationIds = new HashSet<Guid>();
        if (committee.Organization != null)
        {
            organizationIds.Add(committee.Organization.Id);
        }

        if (committee.GetStatus() != ObjectStatus.Created)
        {
            var storedCommittee = (await repository.Find<AdmissionCommittee>(item =>
                item.Id == committee.Id)).FirstOrDefault()
                ?? throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для управления комиссией.");
            if (storedCommittee.Organization != null)
            {
                organizationIds.Add(storedCommittee.Organization.Id);
            }
        }

        var townId = await townProvider.GetCurrentTownId();
        if (!townId.HasValue || organizationIds.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Комиссия недоступна для территории текущего пользователя.");
        }

        foreach (var organizationId in organizationIds)
        {
            var inCurrentTown = await repository.Any<Organization>(organization =>
                organization.Id == organizationId
                && organization.Town.Id == townId.Value);
            if (!inCurrentTown)
            {
                throw new UnauthorizedAccessException(
                    "Комиссия недоступна для территории текущего пользователя.");
            }
        }
    }

    public async Task EnsureCanChange(AdmissionCommitteeEmployee member)
    {
        var committeeIds = new HashSet<Guid>();
        if (member.AdmissionCommittee != null)
        {
            committeeIds.Add(member.AdmissionCommittee.Id);
        }

        if (member.GetStatus() != ObjectStatus.Created)
        {
            var storedMember = (await repository.Find<AdmissionCommitteeEmployee>(item =>
                item.Id == member.Id)).FirstOrDefault()
                ?? throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для управления составом комиссии.");
            if (storedMember.AdmissionCommittee != null)
            {
                committeeIds.Add(storedMember.AdmissionCommittee.Id);
            }
        }

        if (committeeIds.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Не задана комиссия для участника.");
        }

        foreach (var committeeId in committeeIds)
        {
            var committee = (await repository.Find<AdmissionCommittee>(item =>
                item.Id == committeeId)).FirstOrDefault()
                ?? member.AdmissionCommittee;
            if (committee == null || committee.Id != committeeId)
            {
                throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для управления составом комиссии.");
            }

            await EnsureCanChange(committee);
        }
    }
}
