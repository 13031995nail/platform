namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

public sealed class RedinessActAccess(
    IWorkObjectAccessProvider workObjectAccessProvider,
    ActSignAccessValidator actSignAccessValidator,
    IRepository repository)
{
    public async Task<bool> CanCreate(Guid organizationId)
        => await workObjectAccessProvider.CanAccess(
            await GetOrganizationObjectIds([organizationId]),
            WorkObjectRoleNames.Tso);

    public async Task EnsureCanChange(RedinessAct act)
    {
        var objectIds = await GetObjectIds(act);
        if (await workObjectAccessProvider.IsTechnicalAdministrator())
        {
            return;
        }

        if (ActSigningCancellationScope.AllowsDocument(act)
            && act.ActStatus?.Id == IActStatus.Keys.Editing)
        {
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
            return;
        }

        if (BaseActSigningChangeDetector.IsSigningStatusChange(act))
        {
            var signing = await actSignAccessValidator.GetSigningStatus<ReadinessActSign>(
                sign => sign.Act != null && sign.Act.Id == act.Id,
                workObjectAccessProvider.GetCurrentEmployeeId,
                ISignRole.Keys.CommissionChair);
            var storedAct = (await repository.Find<RedinessAct>(item => item.Id == act.Id))
                .FirstOrDefault();
            BaseActBaseBS.EnsureValidStatusTransition(
                act, BaseActBaseBS.GetStoredStatusId(act, storedAct), signing.AllSigned,
                signing.CurrentEmployeeIsBaseSigner);
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
            if (!BaseActSigningChangeDetector.IsIsolatedSigningStatusChange(act))
            {
                await workObjectAccessProvider.EnsureCanAccess(
                    objectIds,
                    WorkObjectRoleNames.Tso);
            }

            return;
        }

        await workObjectAccessProvider.EnsureCanAccess(
            objectIds,
            WorkObjectRoleNames.Tso);
    }

    public async Task EnsureCanChange(RedinessActObject detail)
    {
        await workObjectAccessProvider.EnsureCanAccess(
            detail.WorkObject?.Id,
            detail.GetStatus() == ObjectStatus.Created ? Guid.Empty : detail.Id,
            async () => (await repository.Find<RedinessActObject>(item =>
                item.Id == detail.Id)).FirstOrDefault()?.WorkObject?.Id,
            WorkObjectRoleNames.Tso);

        var acts = new List<RedinessAct>();
        if (detail.Act != null)
        {
            acts.Add(detail.Act);
        }

        if (detail.GetStatus() != ObjectStatus.Created)
        {
            var storedDetail = (await repository.Find<RedinessActObject>(item => item.Id == detail.Id))
                .FirstOrDefault();
            var storedAct = (storedDetail?.GetDataCopy() as RedinessActObject)?.Act
                ?? (ReferenceEquals(detail, storedDetail) ? null : storedDetail?.Act);
            if (storedAct == null)
            {
                throw new UnauthorizedAccessException("Не задан акт готовности.");
            }

            acts.Add(storedAct);
        }

        if (acts.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задан акт готовности.");
        }

        foreach (var act in acts.DistinctBy(item => item.Id))
        {
            await workObjectAccessProvider.EnsureCanAccess(await GetObjectIds(act), WorkObjectRoleNames.Tso);
        }
    }

    public async Task EnsureCanChange(ReadinessActSign sign)
    {
        if (sign.GetStatus() == ObjectStatus.Created && sign.Act != null)
        {
            await workObjectAccessProvider.EnsureCanAccess(
                await GetObjectIds(sign.Act), WorkObjectRoleNames.Tso);
            await actSignAccessValidator.EnsureCanSign(
                sign, nameof(ReadinessActSign.Act),
                workObjectAccessProvider.IsTechnicalAdministrator,
                workObjectAccessProvider.GetCurrentEmployeeId);
            return;
        }

        var actIds = new HashSet<Guid>();
        if (sign.Act != null)
        {
            actIds.Add(sign.Act.Id);
        }

        if (sign.Id != Guid.Empty)
        {
            var storedSigns = await repository.Find<ReadinessActSign>(item =>
                item.Id == sign.Id);
            var storedActId = storedSigns.FirstOrDefault()?.Act?.Id;
            if (!storedActId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для доступа к данным объекта.");
            }

            actIds.Add(storedActId.Value);
        }

        if (actIds.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Недостаточно полномочий для доступа к данным объекта.");
        }

        var objectIds = new HashSet<Guid>();
        var actIdValues = actIds.Cast<object>().ToList();
        var details = await repository.Find<RedinessActObject>(item =>
            actIdValues.Contains(item.Act.Id));
        objectIds.UnionWith(details
            .Where(item => item.WorkObject != null)
            .Select(item => item.WorkObject!.Id));

        if (actSignAccessValidator.IsSigningChange(sign))
        {
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
        }
        else
        {
            await workObjectAccessProvider.EnsureCanAccess(
                objectIds,
                WorkObjectRoleNames.Tso);
        }

        await actSignAccessValidator.EnsureCanSign(
            sign,
            nameof(ReadinessActSign.Act),
            workObjectAccessProvider.IsTechnicalAdministrator,
            workObjectAccessProvider.GetCurrentEmployeeId);
    }

    private async Task<HashSet<Guid>> GetObjectIds(RedinessAct act)
    {
        var objectIds = act.Objects
            .Where(item => item.WorkObject != null)
            .Select(item => item.WorkObject!.Id)
            .ToHashSet();
        var organizationIds = new HashSet<Guid>();
        if (act.Organization != null)
        {
            organizationIds.Add(act.Organization.Id);
        }

        if (act.GetStatus() != ObjectStatus.Created)
        {
            var storedAct = (await repository.Find<RedinessAct>(item =>
                item.Id == act.Id)).FirstOrDefault()
                ?? throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для доступа к данным объекта.");
            if (storedAct.Organization != null)
            {
                organizationIds.Add(storedAct.Organization.Id);
            }

            var storedObjects = await repository.Find<RedinessActObject>(item =>
                item.Act.Id == act.Id);
            objectIds.UnionWith(storedObjects
                .Where(item => item.WorkObject != null)
                .Select(item => item.WorkObject!.Id));
        }

        if (organizationIds.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задана организация документа.");
        }

        objectIds.UnionWith(await GetOrganizationObjectIds(organizationIds));
        return objectIds;
    }

    private async Task<HashSet<Guid>> GetOrganizationObjectIds(IEnumerable<Guid> organizationIds)
    {
        var organizationIdValues = organizationIds.Cast<object>().ToList();
        var objects = await repository.Find<WorkObject>(workObject =>
            workObject.Actuality
            && organizationIdValues.Contains(workObject.Organization.Id));
        return objects.Select(workObject => workObject.Id).ToHashSet();
    }
}

public sealed class RedinessActReadRestrictionProvider(
    IWorkObjectReadLimitProvider readLimitProvider)
    : IODataReadRestrictionProvider
{
    public bool Supports(Type entityType)
        => entityType == typeof(RedinessAct)
            || entityType == typeof(RedinessActObject)
            || entityType == typeof(ReadinessActSign);

    public async Task<ODataReadRestriction> GetRestriction(Type entityType)
        => await WorkObjectODataRestrictionFactory.Create(
            readLimitProvider: readLimitProvider,
            entityType: entityType,
            detailEntityType: typeof(RedinessActObject),
            documentEntityType: typeof(RedinessAct),
            detailView: RedinessActObject.Views.AuditView,
            connectMasterProperty: nameof(RedinessActObject.Act),
            signDocumentProperty: nameof(ReadinessActSign.Act));
}
