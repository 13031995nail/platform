namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

public sealed class AwpReadinessPassportAccess(
    IWorkObjectAccessProvider workObjectAccessProvider,
    ActSignAccessValidator actSignAccessValidator,
    IRepository repository)
{
    public async Task<bool> CanCreate(Guid organizationId, Guid readinessActId)
        => await workObjectAccessProvider.CanAccess(
            await GetCreationObjectIds([organizationId], [readinessActId]),
            WorkObjectRoleNames.Administration);

    public async Task EnsureCanChange(AwpReadinessPassport passport)
    {
        await EnsureCanCreateFromReadinessAct(passport);

        var objectIds = await GetObjectIds(passport);
        if (await workObjectAccessProvider.IsTechnicalAdministrator())
        {
            return;
        }

        if (ActSigningCancellationScope.AllowsDocument(passport)
            && passport.ActStatus?.Id == IActStatus.Keys.Editing)
        {
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
            return;
        }

        if (BaseActSigningChangeDetector.IsSigningStatusChange(passport))
        {
            var signing = await actSignAccessValidator.GetSigningStatus<AwpReadinessPassportSign>(
                sign => sign.Act != null && sign.Act.Id == passport.Id,
                workObjectAccessProvider.GetCurrentEmployeeId,
                ISignRole.Keys.CommissionCreator);
            var storedPassport = (await repository.Find<AwpReadinessPassport>(item =>
                item.Id == passport.Id)).FirstOrDefault();
            BaseActBaseBS.EnsureValidStatusTransition(
                passport, BaseActBaseBS.GetStoredStatusId(passport, storedPassport), signing.AllSigned,
                signing.CurrentEmployeeIsBaseSigner);
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
            if (!BaseActSigningChangeDetector.IsIsolatedSigningStatusChange(passport))
            {
                await workObjectAccessProvider.EnsureCanAccess(
                    objectIds,
                    WorkObjectRoleNames.Administration);
            }

            return;
        }

        await workObjectAccessProvider.EnsureCanAccess(
            objectIds,
            WorkObjectRoleNames.Administration);
    }

    private async Task EnsureCanCreateFromReadinessAct(AwpReadinessPassport passport)
    {
        var readinessActId = passport.ReadinessAct?.Id;
        if (!readinessActId.HasValue || readinessActId.Value == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Не задан акт готовности.");
        }

        var readinessAct = (await repository.Find<RedinessAct>(item => item.Id == readinessActId.Value))
            .FirstOrDefault();
        if (readinessAct == null
            || readinessAct.ActStatus?.Id != IActStatus.Keys.Signed
            || passport.Awp == null
            || readinessAct.Awp == null
            || passport.Awp.Id != readinessAct.Awp.Id
            || passport.Organization == null
            || readinessAct.Organization == null
            || passport.Organization.Id != readinessAct.Organization.Id)
        {
            throw new UnauthorizedAccessException(
                "Паспорт готовности можно создать только по подписанному акту готовности той же организации и АЗП.");
        }
    }

    public async Task EnsureCanChange(AwpReadinessPassportObject detail)
    {
        await workObjectAccessProvider.EnsureCanAccess(
            detail.WorkObject?.Id,
            detail.GetStatus() == ObjectStatus.Created ? Guid.Empty : detail.Id,
            async () => (await repository.Find<AwpReadinessPassportObject>(item =>
                item.Id == detail.Id)).FirstOrDefault()?.WorkObject?.Id,
            WorkObjectRoleNames.Administration);

        var passports = new List<AwpReadinessPassport>();
        if (detail.Act != null)
        {
            passports.Add(detail.Act);
        }

        if (detail.GetStatus() != ObjectStatus.Created)
        {
            var storedDetail = (await repository.Find<AwpReadinessPassportObject>(item => item.Id == detail.Id))
                .FirstOrDefault();
            var storedPassport = (storedDetail?.GetDataCopy() as AwpReadinessPassportObject)?.Act
                ?? (ReferenceEquals(detail, storedDetail) ? null : storedDetail?.Act);
            if (storedPassport == null)
            {
                throw new UnauthorizedAccessException("Не задан паспорт готовности.");
            }

            passports.Add(storedPassport);
        }

        if (passports.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задан паспорт готовности.");
        }

        foreach (var passport in passports.DistinctBy(item => item.Id))
        {
            await workObjectAccessProvider.EnsureCanAccess(
                await GetObjectIds(passport), WorkObjectRoleNames.Administration);
        }
    }

    public async Task EnsureCanChange(AwpReadinessPassportSign sign)
    {
        if (sign.GetStatus() == ObjectStatus.Created && sign.Act != null)
        {
            await workObjectAccessProvider.EnsureCanAccess(
                await GetObjectIds(sign.Act), WorkObjectRoleNames.Administration);
            await actSignAccessValidator.EnsureCanSign(
                sign, nameof(AwpReadinessPassportSign.Act),
                workObjectAccessProvider.IsTechnicalAdministrator,
                workObjectAccessProvider.GetCurrentEmployeeId);
            return;
        }

        var passportIds = new HashSet<Guid>();
        if (sign.Act != null)
        {
            passportIds.Add(sign.Act.Id);
        }

        if (sign.Id != Guid.Empty)
        {
            var storedSigns = await repository.Find<AwpReadinessPassportSign>(item =>
                item.Id == sign.Id);
            var storedPassportId = storedSigns.FirstOrDefault()?.Act?.Id;
            if (!storedPassportId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для доступа к данным объекта.");
            }

            passportIds.Add(storedPassportId.Value);
        }

        if (passportIds.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Недостаточно полномочий для доступа к данным объекта.");
        }

        var objectIds = new HashSet<Guid>();
        var passportIdValues = passportIds.Cast<object>().ToList();
        var details = await repository.Find<AwpReadinessPassportObject>(item =>
            passportIdValues.Contains(item.Act.Id));
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
                WorkObjectRoleNames.Administration);
        }

        await actSignAccessValidator.EnsureCanSign(
            sign,
            nameof(AwpReadinessPassportSign.Act),
            workObjectAccessProvider.IsTechnicalAdministrator,
            workObjectAccessProvider.GetCurrentEmployeeId);
    }

    private async Task<HashSet<Guid>> GetObjectIds(AwpReadinessPassport passport)
    {
        var objectIds = passport.Objects
            .Where(item => item.WorkObject != null)
            .Select(item => item.WorkObject!.Id)
            .ToHashSet();
        var readinessActIds = new HashSet<Guid>();
        var organizationIds = new HashSet<Guid>();
        if (passport.ReadinessAct != null)
        {
            readinessActIds.Add(passport.ReadinessAct.Id);
        }
        if (passport.Organization != null)
        {
            organizationIds.Add(passport.Organization.Id);
        }

        if (passport.GetStatus() != ObjectStatus.Created)
        {
            var storedPassport = (await repository.Find<AwpReadinessPassport>(item =>
                item.Id == passport.Id)).FirstOrDefault()
                ?? throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для доступа к данным объекта.");
            if (storedPassport.ReadinessAct != null)
            {
                readinessActIds.Add(storedPassport.ReadinessAct.Id);
            }
            if (storedPassport.Organization != null)
            {
                organizationIds.Add(storedPassport.Organization.Id);
            }

            var storedObjects = await repository.Find<AwpReadinessPassportObject>(item =>
                item.Act.Id == passport.Id);
            objectIds.UnionWith(storedObjects
                .Where(item => item.WorkObject != null)
                .Select(item => item.WorkObject!.Id));
        }

        if (readinessActIds.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задан акт готовности.");
        }

        objectIds.UnionWith(await GetCreationObjectIds(organizationIds, readinessActIds));
        return objectIds;
    }

    private async Task<HashSet<Guid>> GetCreationObjectIds(
        IEnumerable<Guid> organizationIds,
        IEnumerable<Guid> readinessActIds)
    {
        var readinessActIdValues = readinessActIds.Cast<object>().ToList();
        var readinessObjects = await repository.Find<RedinessActObject>(item =>
            readinessActIdValues.Contains(item.Act.Id));
        var objectIds = new HashSet<Guid>();
        objectIds.UnionWith(readinessObjects
            .Where(item => item.WorkObject != null)
            .Select(item => item.WorkObject!.Id));

        var organizationIdValues = organizationIds.Cast<object>().ToList();
        if (organizationIdValues.Count > 0)
        {
            var organizationObjects = await repository.Find<WorkObject>(workObject =>
                workObject.Actuality
                && organizationIdValues.Contains(workObject.Organization.Id));
            objectIds.UnionWith(organizationObjects.Select(workObject => workObject.Id));
        }

        return objectIds;
    }
}

public sealed class AwpReadinessPassportReadRestrictionProvider(
    IWorkObjectReadLimitProvider readLimitProvider)
    : IODataReadRestrictionProvider
{
    public bool Supports(Type entityType)
        => entityType == typeof(AwpReadinessPassport)
            || entityType == typeof(AwpReadinessPassportObject)
            || entityType == typeof(AwpReadinessPassportSign);

    public async Task<ODataReadRestriction> GetRestriction(Type entityType)
        => await WorkObjectODataRestrictionFactory.Create(
            readLimitProvider: readLimitProvider,
            entityType: entityType,
            detailEntityType: typeof(AwpReadinessPassportObject),
            documentEntityType: typeof(AwpReadinessPassport),
            detailView: AwpReadinessPassportObject.Views.AuditView,
            connectMasterProperty: nameof(AwpReadinessPassportObject.Act),
            signDocumentProperty: nameof(AwpReadinessPassportSign.Act));
}
