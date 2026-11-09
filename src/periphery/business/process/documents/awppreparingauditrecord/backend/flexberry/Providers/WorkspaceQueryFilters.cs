namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Models;

internal static class WorkspaceQueryFilters
{
    public static List<WorkObject> ApplyObjectFilters(List<WorkObject> objects, WorkspaceDashboardQuery query)
    {
        IEnumerable<WorkObject> result = objects;
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            result = result.Where(x =>
                x.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                || x.Organization?.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                || x.AddressString?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
        }
        if (query.TerritoryId.HasValue)
        {
            result = result.Where(x => x.Organization?.Hierarchy?.Id == query.TerritoryId.Value);
        }
        if (query.OrganizationId.HasValue)
        {
            result = result.Where(x => x.Organization?.Id == query.OrganizationId.Value);
        }
        if (query.OrganizationTypeId.HasValue)
        {
            result = result.Where(x => x.Organization?.OrganizationType?.Id == query.OrganizationTypeId.Value);
        }
        if (query.WorkObjectId.HasValue)
        {
            result = result.Where(x => x.Id == query.WorkObjectId.Value);
        }
        return result.ToList();
    }

    public static IReadOnlyList<WorkObjectPreparationInfo> ApplyStateFilters(
        IReadOnlyList<WorkObjectPreparationInfo> objects, WorkspaceDashboardQuery query)
    {
        IEnumerable<WorkObjectPreparationInfo> result = objects;
        if (!string.IsNullOrWhiteSpace(query.State))
        {
            result = result.Where(x => string.Equals(x.State, query.State, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(query.ReadinessLevel))
        {
            result = result.Where(x => string.Equals(GetReadinessKey(x.ReadinessLevel),
                query.ReadinessLevel, StringComparison.OrdinalIgnoreCase));
        }
        return result.ToList();
    }

    public static string GetReadinessKey(LevelReadiness readiness)
        => readiness switch
        {
            LevelReadiness.Ready => "ready",
            LevelReadiness.ReadyWithConditions => "readyWithConditions",
            _ => "notReady"
        };
}
