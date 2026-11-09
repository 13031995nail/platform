namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Process.Documents.Models;
using Tmobis.Core.Repositories;

/// <summary>
/// Выбор доступных объектов и ОЗП без расчёта показателей дашборда.
/// </summary>
public sealed class WorkspaceObjectScopeProvider(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    ICurrentRoleProvider currentRoleProvider,
    EvaluationSheetPermissionProvider permissionProvider)
{
    public async Task<WorkspaceObjectScope> GetScope(WorkspaceDashboardQuery query, bool includeCommission = false)
    {
        var requestedRole = query.Workspace?.Trim().ToLowerInvariant() switch
        {
            "consumer" => WorkObjectRoleNames.Consumer,
            "tso" => WorkObjectRoleNames.Tso,
            "administration" => WorkObjectRoleNames.Administration,
            _ => throw new ArgumentException("Не задано поддерживаемое рабочее пространство.")
        };
        var roleNames = await currentRoleProvider.GetCurrentRoleNames();
        var hasRole = await accessProvider.IsTechnicalAdministrator()
            || roleNames.Contains(requestedRole, StringComparer.OrdinalIgnoreCase);
        var hasCommission = includeCommission && requestedRole == WorkObjectRoleNames.Administration
            && roleNames.Contains(AdmissionCommitteeRoleNames.Member, StringComparer.OrdinalIgnoreCase);
        if (!hasRole && !hasCommission)
        {
            throw new UnauthorizedAccessException();
        }
        var objects = hasRole ? await accessProvider.GetAccessibleWorkObjects(requestedRole) : [];
        var awps = (await repository.Find<Awp>(x => x.Actuality,
                view: Awp.Views.AwpEEntityView))
            .OrderByDescending(x => x.IsCurrent).ThenByDescending(x => x.DateBegin).ToList();
        var awp = query.AwpId.HasValue
            ? awps.FirstOrDefault(x => x.Id == query.AwpId.Value)
            : awps.FirstOrDefault();
        if (query.AwpId.HasValue && awp == null)
        {
            throw new ArgumentException("Выбранный ОЗП не найден или неактуален.");
        }
        if (hasCommission && awp != null)
        {
            objects = objects.Concat(await permissionProvider.GetCommissionObjects(awp.Id))
                .DistinctBy(x => x.Id).ToList();
        }
        return new WorkspaceObjectScope(objects, awps, awp);
    }
}

public sealed record WorkspaceObjectScope(List<WorkObject> Objects, List<Awp> Awps, Awp? Awp);
