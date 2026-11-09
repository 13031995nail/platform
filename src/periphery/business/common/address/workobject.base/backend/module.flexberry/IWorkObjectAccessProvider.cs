namespace Tmobis.Business.Common.Address;

/// <summary>
/// Проверка доступа текущего пользователя к объектам работ.
/// </summary>
public interface IWorkObjectAccessProvider
{
    Task<bool> IsTechnicalAdministrator();

    Task<Guid?> GetCurrentEmployeeId();

    /// <summary>
    /// Проверка ролевого доступа по объекту, загруженному с представлением WorkObjectE.
    /// </summary>
    Task<WorkObjectRoleAccess> GetRoleAccess(WorkObject workObject);

    Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(IEnumerable<Guid> workObjectIds);

    Task<IReadOnlySet<Guid>> GetAccessibleWorkObjectIds(IEnumerable<Guid> workObjectIds, string role);

    Task<List<WorkObject>> GetAccessibleWorkObjects();

    Task<List<WorkObject>> GetAccessibleWorkObjects(string role);

    Task EnsureCanAccess(Guid workObjectId);

    Task EnsureCanAccess(
        Guid? loadedWorkObjectId,
        Guid entityId,
        Func<Task<Guid?>> getStoredWorkObjectId);

    Task EnsureCanAccess(IEnumerable<Guid> workObjectIds, bool allowEmpty = false);

    Task<bool> CanAccess(IEnumerable<Guid> workObjectIds, string role);

    Task EnsureCanAccess(
        IEnumerable<Guid> workObjectIds,
        string role,
        bool allowEmpty = false);

    Task EnsureCanAccess(
        Guid? loadedWorkObjectId,
        Guid entityId,
        Func<Task<Guid?>> getStoredWorkObjectId,
        string role);

    Task EnsureCanCreateWorkObject(Guid organizationId);

    Task EnsureTechnicalAdministrator();
}

public sealed record WorkObjectRoleAccess(bool Consumer, bool Tso, bool Administration);
