namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.FunctionalLanguage;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

/// <inheritdoc />
public sealed class WorkObjectAccessProvider(
    ICurrentUserProvider currentUserProvider,
    ICurrentRoleProvider currentRoleProvider,
    IRepository repository,
    IDataService dataService)
    : IWorkObjectAccessProvider, IWorkObjectReadLimitProvider
{
    private bool contextLoaded;
    private HashSet<string> roleNames = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<Guid> organizationIds = [];
    private Guid? townId;
    private Guid? employeeId;

    /// <inheritdoc />
    public async Task<bool> IsTechnicalAdministrator()
    {
        await EnsureContextLoaded();
        return roleNames.Contains(WorkObjectRoleNames.Admin)
            || roleNames.Contains(WorkObjectRoleNames.Administrator);
    }

    /// <inheritdoc />
    public async Task<Guid?> GetCurrentEmployeeId()
    {
        await EnsureContextLoaded();
        return employeeId;
    }

    public async Task<WorkObjectRoleAccess> GetRoleAccess(WorkObject workObject)
    {
        await EnsureContextLoaded();
        var consumer = roleNames.Contains(WorkObjectRoleNames.Consumer)
            && organizationIds.Contains(workObject.Organization.Id);
        var administration = roleNames.Contains(WorkObjectRoleNames.Administration)
            && townId.HasValue && workObject.Organization.Town?.Id == townId.Value;
        var organizations = organizationIds.Cast<object>().ToList();
        var tso = roleNames.Contains(WorkObjectRoleNames.Tso) && organizations.Count > 0
            && await repository.Any<WorkObjectTsoOrganization>(link =>
                link.WorkObject.Id == workObject.Id && organizations.Contains(link.Organization.Id));
        return new WorkObjectRoleAccess(consumer, tso, administration);
    }

    /// <inheritdoc />
    public async Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(
        IEnumerable<Guid> workObjectIds)
        => await GetAccessibleWorkObjectIds(workObjectIds, null);

    /// <inheritdoc />
    public async Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(
        IEnumerable<Guid> workObjectIds, string? role)
    {
        var requestedIds = workObjectIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
        if (requestedIds.Count == 0)
        {
            return new HashSet<Guid>();
        }

        if (await IsTechnicalAdministrator())
        {
            return requestedIds.ToHashSet();
        }

        await EnsureContextLoaded();
        var result = new HashSet<Guid>();
        var requestedIdValues = requestedIds.Cast<object>().ToList();
        var organizations = organizationIds.Cast<object>().ToList();

        if ((role == null || role == WorkObjectRoleNames.Consumer)
            && roleNames.Contains(WorkObjectRoleNames.Consumer) && organizations.Count > 0)
        {
            var consumerObjects = await repository.Find<WorkObject>(workObject =>
                requestedIdValues.Contains(workObject.Id)
                && organizations.Contains(workObject.Organization.Id));
            result.UnionWith(consumerObjects.Select(workObject => workObject.Id));
        }

        if ((role == null || role == WorkObjectRoleNames.Tso)
            && roleNames.Contains(WorkObjectRoleNames.Tso) && organizations.Count > 0)
        {
            var tsoLinks = await repository.Find<WorkObjectTsoOrganization>(link =>
                requestedIdValues.Contains(link.WorkObject.Id)
                && organizations.Contains(link.Organization.Id));
            result.UnionWith(tsoLinks.Select(link => link.WorkObject.Id));
        }

        if ((role == null || role == WorkObjectRoleNames.Administration)
            && roleNames.Contains(WorkObjectRoleNames.Administration) && townId.HasValue)
        {
            var currentTownId = townId.Value;
            var administrationObjects = await repository.Find<WorkObject>(workObject =>
                requestedIdValues.Contains(workObject.Id)
                && workObject.Organization.Town.Id == currentTownId);
            result.UnionWith(administrationObjects.Select(workObject => workObject.Id));
        }

        return result;
    }

    /// <inheritdoc />
    public Task<List<WorkObject>> GetAccessibleWorkObjects()
        => GetAccessibleWorkObjects(null);

    /// <inheritdoc />
    public async Task<List<WorkObject>> GetAccessibleWorkObjects(string? role)
    {
        await EnsureContextLoaded();
        if (roleNames.Contains(WorkObjectRoleNames.Admin)
            || roleNames.Contains(WorkObjectRoleNames.Administrator))
        {
            return await repository.Find<WorkObject>(
                workObject => workObject.Actuality,
                view: WorkObject.Views.WorkObjectEEntityView);
        }

        var result = new Dictionary<Guid, WorkObject>();
        var organizations = organizationIds.Cast<object>().ToList();

        if ((role == null || role == WorkObjectRoleNames.Consumer)
            && roleNames.Contains(WorkObjectRoleNames.Consumer) && organizations.Count > 0)
        {
            var consumerObjects = await repository.Find<WorkObject>(
                workObject => workObject.Actuality
                    && organizations.Contains(workObject.Organization.Id),
                view: WorkObject.Views.WorkObjectEEntityView);
            foreach (var workObject in consumerObjects)
            {
                result[workObject.Id] = workObject;
            }
        }

        if ((role == null || role == WorkObjectRoleNames.Tso)
            && roleNames.Contains(WorkObjectRoleNames.Tso) && organizations.Count > 0)
        {
            var tsoLinks = await repository.Find<WorkObjectTsoOrganization>(
                link => organizations.Contains(link.Organization.Id),
                view: WorkObjectTsoOrganization.Views.WorkObjectTsoOrganizationLEntityView);
            var workObjectIds = tsoLinks
                .Select(link => link.WorkObject.Id)
                .Distinct()
                .Cast<object>()
                .ToList();
            foreach (var workObjectIdBatch in workObjectIds.Chunk(500))
            {
                var batchIds = workObjectIdBatch.ToList();
                var tsoObjects = await repository.Find<WorkObject>(
                    workObject => workObject.Actuality
                        && batchIds.Contains(workObject.Id),
                    view: WorkObject.Views.WorkObjectEEntityView);
                foreach (var workObject in tsoObjects)
                {
                    result[workObject.Id] = workObject;
                }
            }
        }

        if ((role == null || role == WorkObjectRoleNames.Administration)
            && roleNames.Contains(WorkObjectRoleNames.Administration) && townId.HasValue)
        {
            var currentTownId = townId.Value;
            var administrationObjects = await repository.Find<WorkObject>(
                workObject => workObject.Actuality
                    && workObject.Organization.Town.Id == currentTownId,
                view: WorkObject.Views.WorkObjectEEntityView);
            foreach (var workObject in administrationObjects)
            {
                result[workObject.Id] = workObject;
            }
        }

        return result.Values.ToList();
    }

    /// <inheritdoc />
    public async Task<Function?> GetReadLimit(
        string workObjectPropertyPath)
    {
        await EnsureContextLoaded();
        if (roleNames.Contains(WorkObjectRoleNames.Admin)
            || roleNames.Contains(WorkObjectRoleNames.Administrator))
        {
            return null;
        }

        var language = new ICSSoft.STORMNET.Windows.Forms.ExternalLangDef(dataService);
        var organizations = organizationIds.Cast<object>().ToArray();
        var restrictions = new List<Function>();
        var prefix = workObjectPropertyPath == "STORMMainObjectKey"
            ? string.Empty
            : $"{workObjectPropertyPath}.";

        if (roleNames.Contains(WorkObjectRoleNames.Consumer) && organizations.Length > 0)
        {
            restrictions.Add(FunctionBuilder.BuildIn(
                new VariableDef(language.GuidType, $"{prefix}Organization"),
                organizations));
        }

        if (roleNames.Contains(WorkObjectRoleNames.Tso) && organizations.Length > 0)
        {
            var tsoLinks = await repository.Find<WorkObjectTsoOrganization>(
                link => organizations.Contains(link.Organization.Id),
                view: WorkObjectTsoOrganization.Views.WorkObjectTsoOrganizationLEntityView);
            var workObjectIds = tsoLinks
                .Select(link => link.WorkObject.Id)
                .Distinct()
                .Cast<object>()
                .ToArray();
            if (workObjectIds.Length > 0)
            {
                restrictions.Add(FunctionBuilder.BuildIn(
                    new VariableDef(language.GuidType, workObjectPropertyPath),
                    workObjectIds));
            }
        }

        if (roleNames.Contains(WorkObjectRoleNames.Administration) && townId.HasValue)
        {
            restrictions.Add(FunctionBuilder.BuildEquals(
                new VariableDef(language.GuidType, $"{prefix}Organization.Town"),
                townId.Value));
        }

        var limitFunction = restrictions.Count == 0
            ? FunctionBuilder.BuildEquals(
                new VariableDef(language.GuidType, workObjectPropertyPath),
                Guid.Empty)
            : FunctionBuilder.BuildOr(restrictions);
        return limitFunction;
    }

    /// <inheritdoc />
    public async Task<Function?> GetReadLimit(
        View detailView,
        string connectMasterProperty,
        string ownerConnectProperty)
    {
        var allowedDetailLimit = await GetReadLimit("WorkObject");
        if (allowedDetailLimit == null)
        {
            return null;
        }

        var language = new ICSSoft.STORMNET.Windows.Forms.ExternalLangDef(dataService);
        var detailVariable = FunctionHelper.GetDetailVarDef(
            detailView,
            connectMasterProperty,
            [ownerConnectProperty]);
        var hasAllowedDetail = FunctionBuilder.BuildExists(
            detailVariable,
            allowedDetailLimit);
        var deniedDetailLimit = language.GetFunction(
            language.funcNOT,
            allowedDetailLimit);
        var hasDeniedDetail = FunctionBuilder.BuildExists(
            detailVariable,
            deniedDetailLimit);
        return FunctionBuilder.BuildAnd(
            hasAllowedDetail,
            language.GetFunction(language.funcNOT, hasDeniedDetail));
    }

    /// <inheritdoc />
    public Task EnsureCanAccess(Guid workObjectId)
        => EnsureCanAccess([workObjectId]);

    /// <inheritdoc />
    public async Task EnsureCanAccess(
        Guid? loadedWorkObjectId,
        Guid entityId,
        Func<Task<Guid?>> getStoredWorkObjectId)
        => await EnsureCanAccessForEntity(
            loadedWorkObjectId,
            entityId,
            getStoredWorkObjectId,
            role: null);

    /// <inheritdoc />
    public async Task EnsureCanAccess(
        Guid? loadedWorkObjectId,
        Guid entityId,
        Func<Task<Guid?>> getStoredWorkObjectId,
        string role)
        => await EnsureCanAccessForEntity(
            loadedWorkObjectId,
            entityId,
            getStoredWorkObjectId,
            role);

    private async Task EnsureCanAccessForEntity(
        Guid? loadedWorkObjectId,
        Guid entityId,
        Func<Task<Guid?>> getStoredWorkObjectId,
        string? role)
    {
        var objectIds = new HashSet<Guid>();
        if (loadedWorkObjectId.HasValue)
        {
            objectIds.Add(loadedWorkObjectId.Value);
        }

        if (entityId == Guid.Empty)
        {
            if (!loadedWorkObjectId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Недостаточно полномочий для доступа к данным объекта.");
            }

            if (role == null)
            {
                await EnsureCanAccess(objectIds);
            }
            else
            {
                await EnsureCanAccess(objectIds, role);
            }
            return;
        }

        var storedWorkObjectId = await getStoredWorkObjectId();
        if (!storedWorkObjectId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Недостаточно полномочий для доступа к данным объекта.");
        }

        objectIds.Add(storedWorkObjectId.Value);
        if (role == null)
        {
            await EnsureCanAccess(objectIds);
        }
        else
        {
            await EnsureCanAccess(objectIds, role);
        }
    }

    /// <inheritdoc />
    public Task EnsureCanAccess(
        IEnumerable<Guid> workObjectIds,
        bool allowEmpty = false)
        => EnsureCanAccessCore(workObjectIds, role: null, allowEmpty, allowTechnicalAdministrator: true);

    /// <inheritdoc />
    public async Task<bool> CanAccess(IEnumerable<Guid> workObjectIds, string role)
    {
        var requestedIds = workObjectIds.Where(id => id != Guid.Empty).Distinct().ToList();
        if (requestedIds.Count == 0)
        {
            return false;
        }

        var allowedIds = await GetAccessibleWorkObjectIds(requestedIds, role);
        return requestedIds.All(allowedIds.Contains);
    }

    /// <inheritdoc />
    public Task EnsureCanAccess(
        IEnumerable<Guid> workObjectIds,
        string role,
        bool allowEmpty = false)
        => EnsureCanAccessCore(workObjectIds, role, allowEmpty, allowTechnicalAdministrator: false);

    private async Task EnsureCanAccessCore(
        IEnumerable<Guid> workObjectIds,
        string? role,
        bool allowEmpty,
        bool allowTechnicalAdministrator)
    {
        if (allowTechnicalAdministrator && await IsTechnicalAdministrator())
        {
            return;
        }

        var requestedIds = workObjectIds.Where(id => id != Guid.Empty).Distinct().ToList();
        var allowedIds = role == null
            ? await GetAccessibleWorkObjectIds(requestedIds)
            : await GetAccessibleWorkObjectIds(requestedIds, role);
        if ((!allowEmpty && requestedIds.Count == 0)
            || requestedIds.Any(id => !allowedIds.Contains(id)))
        {
            throw new UnauthorizedAccessException(
                "Недостаточно полномочий для доступа к данным объекта.");
        }
    }

    /// <inheritdoc />
    public async Task EnsureCanCreateWorkObject(Guid organizationId)
    {
        if (await IsTechnicalAdministrator())
        {
            return;
        }

        await EnsureContextLoaded();
        if (!roleNames.Contains(WorkObjectRoleNames.Consumer)
            || !organizationIds.Contains(organizationId))
        {
            throw new UnauthorizedAccessException(
                "Потребитель может создавать объекты только своей организации.");
        }
    }

    /// <inheritdoc />
    public async Task EnsureTechnicalAdministrator()
    {
        if (!await IsTechnicalAdministrator())
        {
            throw new UnauthorizedAccessException(
                "Изменять связи объектов с ТСО может только технический администратор.");
        }
    }

    private async Task EnsureContextLoaded()
    {
        if (contextLoaded)
        {
            return;
        }

        roleNames = new HashSet<string>(
            await currentRoleProvider.GetCurrentRoleNames(),
            StringComparer.OrdinalIgnoreCase);
        var employee = await currentUserProvider.GetCurrentEmployee();
        var user = await currentUserProvider.GetCurrentUser();

        if (employee != null)
        {
            employeeId = employee.Id;
            var places = await repository.Find<PlaceOfEmployment>(place =>
                place.Employee.Id == employee.Id);
            organizationIds.UnionWith(places.Select(place => place.Organization.Id));
        }

        townId = user?.Town?.Id;
        contextLoaded = true;
    }
}
