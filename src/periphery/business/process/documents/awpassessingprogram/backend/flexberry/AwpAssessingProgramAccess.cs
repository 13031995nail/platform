namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

public sealed class AwpAssessingProgramAccess(
    IWorkObjectAccessProvider workObjectAccessProvider,
    ActSignAccessValidator actSignAccessValidator,
    IRepository repository)
{
    public async Task EnsureCanChange(AwpAssessingProgram program)
    {
        var objectIds = await GetObjectIds(program);
        if (await workObjectAccessProvider.IsTechnicalAdministrator())
        {
            return;
        }

        if (ActSigningCancellationScope.AllowsDocument(program)
            && program.ActStatus?.Id == IActStatus.Keys.Editing)
        {
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
            return;
        }

        if (BaseActSigningChangeDetector.IsSigningStatusChange(program))
        {
            var signing = await actSignAccessValidator.GetSigningStatus<AwpAssessingProgramSign>(
                sign => sign.Act != null && sign.Act.Id == program.Id,
                workObjectAccessProvider.GetCurrentEmployeeId,
                ISignRole.Keys.CommissionChair);
            var storedProgram = (await repository.Find<AwpAssessingProgram>(item =>
                item.Id == program.Id)).FirstOrDefault();
            BaseActBaseBS.EnsureValidStatusTransition(
                program, BaseActBaseBS.GetStoredStatusId(program, storedProgram), signing.AllSigned,
                signing.CurrentEmployeeIsBaseSigner, allowApproval: true);
            await workObjectAccessProvider.EnsureCanAccess(objectIds);
            if (!BaseActSigningChangeDetector.IsIsolatedSigningStatusChange(
                program,
                nameof(program.ApprovedDate)))
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

    public async Task EnsureCanChange(AwpAssessingProgramObject detail)
    {
        await workObjectAccessProvider.EnsureCanAccess(
            detail.WorkObject?.Id,
            detail.GetStatus() == ObjectStatus.Created ? Guid.Empty : detail.Id,
            async () => (await repository.Find<AwpAssessingProgramObject>(item =>
                item.Id == detail.Id)).FirstOrDefault()?.WorkObject?.Id,
            WorkObjectRoleNames.Administration);

        var programs = new List<AwpAssessingProgram>();
        if (detail.Program != null)
        {
            programs.Add(detail.Program);
        }

        if (detail.GetStatus() != ObjectStatus.Created)
        {
            var storedDetail = (await repository.Find<AwpAssessingProgramObject>(item => item.Id == detail.Id))
                .FirstOrDefault();
            var storedProgram = (storedDetail?.GetDataCopy() as AwpAssessingProgramObject)?.Program
                ?? (ReferenceEquals(detail, storedDetail) ? null : storedDetail?.Program);
            if (storedProgram == null)
            {
                throw new UnauthorizedAccessException("Не задана программа оценки.");
            }

            programs.Add(storedProgram);
        }

        if (programs.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задана программа оценки.");
        }

        foreach (var program in programs.DistinctBy(item => item.Id))
        {
            await workObjectAccessProvider.EnsureCanAccess(
                await GetObjectIds(program), WorkObjectRoleNames.Administration);
        }
    }

    public async Task EnsureCanChange(AwpAssessingProgramEvaluationSchedule schedule)
    {
        var programs = new List<AwpAssessingProgram>();
        if (schedule.Program != null)
        {
            programs.Add(schedule.Program);
        }

        if (schedule.GetStatus() != ObjectStatus.Created)
        {
            var storedSchedule = (await repository.Find<AwpAssessingProgramEvaluationSchedule>(
                item => item.Id == schedule.Id)).FirstOrDefault();
            var storedProgram = (storedSchedule?.GetDataCopy() as AwpAssessingProgramEvaluationSchedule)?.Program
                ?? (ReferenceEquals(schedule, storedSchedule) ? null : storedSchedule?.Program);
            if (storedProgram == null)
            {
                throw new UnauthorizedAccessException("Не задана программа оценки.");
            }

            programs.Add(storedProgram);
        }

        if (programs.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задана программа оценки.");
        }

        var objectIds = new HashSet<Guid>();
        foreach (var program in programs.DistinctBy(item => item.Id))
        {
            objectIds.UnionWith(await GetObjectIds(program));
        }

        await workObjectAccessProvider.EnsureCanAccess(objectIds, WorkObjectRoleNames.Administration);
    }

    public async Task EnsureCanChange(AwpAssessingProgramSign sign)
    {
        if (sign.GetStatus() == ObjectStatus.Created && sign.Act != null)
        {
            await workObjectAccessProvider.EnsureCanAccess(
                await GetObjectIds(sign.Act), WorkObjectRoleNames.Administration);
            await actSignAccessValidator.EnsureCanSign(
                sign, nameof(AwpAssessingProgramSign.Act),
                workObjectAccessProvider.IsTechnicalAdministrator,
                workObjectAccessProvider.GetCurrentEmployeeId);
            return;
        }

        var programIds = new HashSet<Guid>();
        if (sign.Act != null)
        {
            programIds.Add(sign.Act.Id);
        }

        if (sign.Id != Guid.Empty)
        {
            var storedSigns = await repository.Find<AwpAssessingProgramSign>(item =>
                item.Id == sign.Id);
            var storedProgramId = storedSigns.FirstOrDefault()?.Act?.Id;
            if (!storedProgramId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для доступа к данным объекта.");
            }

            programIds.Add(storedProgramId.Value);
        }

        if (programIds.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Недостаточно полномочий для доступа к данным объекта.");
        }

        var objectIds = new HashSet<Guid>();
        var organizationIds = new HashSet<Guid>();
        await AddStoredProgramObjects(programIds, objectIds, organizationIds);
        await AddOrganizationObjects(organizationIds, objectIds);

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
            nameof(AwpAssessingProgramSign.Act),
            workObjectAccessProvider.IsTechnicalAdministrator,
            workObjectAccessProvider.GetCurrentEmployeeId);
    }

    private async Task<HashSet<Guid>> GetObjectIds(AwpAssessingProgram program)
    {
        var objectIds = GetLoadedObjectIds(program);
        var organizationIds = new HashSet<Guid>();
        if (program.Organization != null)
        {
            organizationIds.Add(program.Organization.Id);
        }

        if (program.GetStatus() != ObjectStatus.Created)
        {
            await AddStoredProgramObjects([program.Id], objectIds, organizationIds);
        }

        if (organizationIds.Count == 0)
        {
            throw new UnauthorizedAccessException("Не задана организация документа.");
        }

        await AddOrganizationObjects(organizationIds, objectIds);
        return objectIds;
    }

    private async Task AddStoredProgramObjects(
        IEnumerable<Guid> programIds,
        HashSet<Guid> objectIds,
        HashSet<Guid> organizationIds)
    {
        var requestedIds = programIds.Distinct().ToList();
        var programIdValues = requestedIds.Cast<object>().ToList();
        var storedPrograms = await repository.Find<AwpAssessingProgram>(item =>
            programIdValues.Contains(item.Id),
            view: AwpAssessingProgram.Views.AwpAssessingProgramLEntityView);
        var storedById = storedPrograms.ToDictionary(item => item.Id);
        foreach (var programId in requestedIds)
        {
            if (!storedById.TryGetValue(programId, out var program)
                || program.Organization == null)
            {
                throw new UnauthorizedAccessException("Не задана организация документа.");
            }

            organizationIds.Add(program.Organization.Id);
            objectIds.UnionWith(GetLoadedObjectIds(program));
        }

        var storedObjects = await repository.Find<AwpAssessingProgramObject>(item =>
            programIdValues.Contains(item.Program.Id));
        objectIds.UnionWith(storedObjects
            .Where(item => item.WorkObject != null)
            .Select(item => item.WorkObject!.Id));
    }

    private async Task AddOrganizationObjects(
        IEnumerable<Guid> organizationIds,
        HashSet<Guid> objectIds)
    {
        var organizationIdValues = organizationIds.Cast<object>().ToList();
        var objects = await repository.Find<WorkObject>(workObject =>
            organizationIdValues.Contains(workObject.Organization.Id)
            || (workObject.Organization.Hierarchy != null
                && organizationIdValues.Contains(workObject.Organization.Hierarchy.Id)));
        objectIds.UnionWith(objects.Select(workObject => workObject.Id));
    }

    private HashSet<Guid> GetLoadedObjectIds(AwpAssessingProgram program)
        => program.Objects
            .Where(item => item.WorkObject != null)
            .Select(item => item.WorkObject!.Id)
            .ToHashSet();
}

public sealed class AwpAssessingProgramReadRestrictionProvider(
    IWorkObjectReadLimitProvider readLimitProvider)
    : IODataReadRestrictionProvider
{
    public bool Supports(Type entityType)
        => entityType == typeof(AwpAssessingProgram)
            || entityType == typeof(AwpAssessingProgramObject)
            || entityType == typeof(AwpAssessingProgramEvaluationSchedule)
            || entityType == typeof(AwpAssessingProgramSign);

    public async Task<ODataReadRestriction> GetRestriction(Type entityType)
        => await WorkObjectODataRestrictionFactory.Create(
            readLimitProvider: readLimitProvider,
            entityType: entityType,
            detailEntityType: typeof(AwpAssessingProgramObject),
            documentEntityType: typeof(AwpAssessingProgram),
            detailView: AwpAssessingProgramObject.Views.AuditView,
            connectMasterProperty: nameof(AwpAssessingProgramObject.Program),
            signDocumentProperty: entityType == typeof(AwpAssessingProgramEvaluationSchedule)
                ? nameof(AwpAssessingProgramEvaluationSchedule.Program)
                : nameof(AwpAssessingProgramSign.Act));
}
