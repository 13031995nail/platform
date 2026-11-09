namespace Tmobis.Business.Process.Documents.Tests;

using System.Reflection;
using Tmobis.Business.Common.Address;
using Tmobis.Core.Repositories;

internal static class TestRepository
{
    public static IRepository Create(
        Func<MethodInfo, object?[]?, object?> handler)
    {
        var repository = DispatchProxy.Create<IRepository, RepositoryProxy>();
        ((RepositoryProxy)repository).Handler = handler;
        return repository;
    }

    public class RepositoryProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => Handler(targetMethod!, args);
    }
}

internal sealed class TestWorkObjectAccessProvider(
    IEnumerable<Guid>? genericIds = null)
    : IWorkObjectAccessProvider
{
    public IReadOnlySet<Guid> GenericIds { get; } =
        genericIds?.ToHashSet() ?? new HashSet<Guid>();

    public Dictionary<string, IReadOnlySet<Guid>> RoleIds { get; } = [];

    public bool TechnicalAdministrator { get; init; }

    public Guid? CurrentEmployeeId { get; set; }

    public Task<bool> IsTechnicalAdministrator()
        => Task.FromResult(TechnicalAdministrator);

    public Task<Guid?> GetCurrentEmployeeId()
        => Task.FromResult(CurrentEmployeeId);

    public Task<WorkObjectRoleAccess> GetRoleAccess(WorkObject workObject)
        => Task.FromResult(new WorkObjectRoleAccess(
            HasRoleAccess(WorkObjectRoleNames.Consumer, workObject.Id),
            HasRoleAccess(WorkObjectRoleNames.Tso, workObject.Id),
            HasRoleAccess(WorkObjectRoleNames.Administration, workObject.Id)));

    public Task<List<WorkObject>> GetAccessibleWorkObjects()
        => throw new NotSupportedException();

    public Task<List<WorkObject>> GetAccessibleWorkObjects(string role)
        => throw new NotSupportedException();

    public Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(
        IEnumerable<Guid> ids)
        => Task.FromResult<IReadOnlySet<Guid>>(
            ids.Where(GenericIds.Contains).ToHashSet());

    public Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(
        IEnumerable<Guid> ids,
        string role)
        => Task.FromResult<IReadOnlySet<Guid>>(
            RoleIds.TryGetValue(role, out var allowed)
                ? ids.Where(allowed.Contains).ToHashSet()
                : new HashSet<Guid>());

    public Task EnsureCanAccess(Guid id)
        => EnsureCanAccess([id]);

    public async Task<bool> CanAccess(IEnumerable<Guid> ids, string role)
    {
        var requested = ids.Where(id => id != Guid.Empty).Distinct().ToList();
        if (requested.Count == 0)
        {
            return false;
        }

        var allowed = await GetAccessibleWorkObjectIds(requested, role);
        return requested.All(allowed.Contains);
    }

    public async Task EnsureCanAccess(
        Guid? loadedId,
        Guid entityId,
        Func<Task<Guid?>> storedId,
        string role)
    {
        var ids = new HashSet<Guid>();
        if (loadedId.HasValue)
        {
            ids.Add(loadedId.Value);
        }

        if (entityId != Guid.Empty)
        {
            var stored = await storedId();
            if (!stored.HasValue)
            {
                throw new UnauthorizedAccessException();
            }

            ids.Add(stored.Value);
        }

        await EnsureCanAccess(ids, role);
    }

    public async Task EnsureCanAccess(
        IEnumerable<Guid> ids,
        string role,
        bool allowEmpty = false)
    {
        var requested = ids.Where(id => id != Guid.Empty).Distinct().ToList();
        var allowed = await GetAccessibleWorkObjectIds(requested, role);
        if ((!allowEmpty && requested.Count == 0)
            || requested.Any(id => !allowed.Contains(id)))
        {
            throw new UnauthorizedAccessException();
        }
    }

    public async Task EnsureCanAccess(
        Guid? loadedId,
        Guid entityId,
        Func<Task<Guid?>> storedId)
    {
        var ids = new HashSet<Guid>();
        if (loadedId.HasValue)
        {
            ids.Add(loadedId.Value);
        }

        if (entityId != Guid.Empty)
        {
            var stored = await storedId();
            if (stored.HasValue)
            {
                ids.Add(stored.Value);
            }
        }

        await EnsureCanAccess(ids);
    }

    public Task EnsureCanAccess(
        IEnumerable<Guid> ids,
        bool allowEmpty = false)
    {
        var requested = ids.Where(id => id != Guid.Empty).Distinct().ToList();
        if ((!allowEmpty && requested.Count == 0)
            || requested.Any(id => !GenericIds.Contains(id)))
        {
            throw new UnauthorizedAccessException();
        }

        return Task.CompletedTask;
    }

    public Task EnsureCanCreateWorkObject(Guid organizationId)
        => Task.CompletedTask;

    public Task EnsureTechnicalAdministrator()
        => TechnicalAdministrator
            ? Task.CompletedTask
            : throw new UnauthorizedAccessException();

    private bool HasRoleAccess(string role, Guid workObjectId)
        => RoleIds.TryGetValue(role, out var allowed) && allowed.Contains(workObjectId);
}
