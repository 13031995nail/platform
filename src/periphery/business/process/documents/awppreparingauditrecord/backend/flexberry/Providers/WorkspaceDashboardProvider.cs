namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Models;

/// <summary>
/// Формирование сводки по фактическому состоянию доступных объектов.
/// </summary>
public sealed class WorkspaceDashboardProvider(
    WorkspaceObjectScopeProvider scopeProvider,
    IAwpPreparingAuditRecordProvider awpPreparingAuditRecordProvider) : IWorkspaceDashboardProvider
{
    public async Task<WorkspaceDashboard> GetDashboard(WorkspaceDashboardQuery query)
    {
        var scope = await scopeProvider.GetScope(query);
        var awp = scope.Awp;
        var filters = BuildFilterOptions(scope.Objects, scope.Awps);
        var objects = WorkspaceQueryFilters.ApplyObjectFilters(scope.Objects, query);

        var objectStates = await awpPreparingAuditRecordProvider
            .GetObjectPreparationInfo(
                awp?.Id,
                objects.Select(workObject => workObject.Id).ToList());

        var statistics = BuildStatistics(objectStates);
        objectStates = WorkspaceQueryFilters.ApplyStateFilters(objectStates, query);

        var objectsById = objects.ToDictionary(x => x.Id);
        return new WorkspaceDashboard(
            query.Workspace!,
            awp?.Id,
            awp == null ? null : GetAwpCaption(awp),
            statistics,
            filters,
            objectStates.Select(x => new WorkspaceObject(
                x.WorkObjectId,
                objectsById[x.WorkObjectId].Name,
                x.State,
                WorkspaceQueryFilters.GetReadinessKey(x.ReadinessLevel),
                x.HasMissingDocuments,
                x.HasOverdueRemarks,
                x.Progress?.Percent,
                objectsById[x.WorkObjectId].Organization?.Name,
                objectsById[x.WorkObjectId].AddressString)).ToList());
    }

    private static WorkspaceStatistics BuildStatistics(
        IReadOnlyList<WorkObjectPreparationInfo> objects)
    {
        return new WorkspaceStatistics(
            objects.Count,
            objects.Count(x => x.State == WorkObjectPreparationStates.NotStarted),
            objects.Count(x => x.State == WorkObjectPreparationStates.InProgress),
            objects.Count(x => x.State == WorkObjectPreparationStates.UnderReview),
            objects.Count(x => x.State == WorkObjectPreparationStates.Remarks),
            objects.Count(x => x.State == WorkObjectPreparationStates.Ready),
            objects.Count(x => x.State == WorkObjectPreparationStates.Completed),
            objects.Count(x => x.ReadinessLevel == LevelReadiness.Ready),
            objects.Count(x => x.ReadinessLevel == LevelReadiness.ReadyWithConditions),
            objects.Count(x => x.ReadinessLevel == LevelReadiness.NotReady),
            objects.Count(x => x.HasMissingDocuments),
            objects.Count(x => x.HasOverdueRemarks),
            objects.Count(x => x.State is WorkObjectPreparationStates.UnderReview
                or WorkObjectPreparationStates.Remarks
                || x.HasMissingDocuments
                || x.HasOverdueRemarks));
    }

    private static WorkspaceFilterOptions BuildFilterOptions(
        List<WorkObject> objects,
        List<Awp> awps)
    {
        var organizations = objects
            .Where(x => x.Organization != null)
            .Select(x => x.Organization)
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .ToList();

        return new WorkspaceFilterOptions(
            awps.Select(x => new WorkspaceOption(x.Id, GetAwpCaption(x)))
                .ToList(),
            organizations
                .Where(x => x.Hierarchy != null)
                .Select(x => x.Hierarchy!)
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .Select(x => new WorkspaceOption(
                    x.Id,
                    string.IsNullOrWhiteSpace(x.Acronym) ? x.Name : x.Acronym))
                .OrderBy(x => x.Caption)
                .ToList(),
            organizations
                .Select(x => new WorkspaceOption(x.Id, x.Name))
                .OrderBy(x => x.Caption)
                .ToList(),
            organizations
                .Where(x => x.OrganizationType != null)
                .Select(x => x.OrganizationType)
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .Select(x => new WorkspaceOption(x.Id, x.Name))
                .OrderBy(x => x.Caption)
                .ToList(),
            objects
                .Select(x => new WorkspaceOption(x.Id, x.Name))
                .OrderBy(x => x.Caption)
                .ToList(),
            [
                new WorkspaceTextOption(WorkObjectPreparationStates.NotStarted, "Не начато"),
                new WorkspaceTextOption(WorkObjectPreparationStates.InProgress, "В работе"),
                new WorkspaceTextOption(WorkObjectPreparationStates.UnderReview, "На проверке"),
                new WorkspaceTextOption(WorkObjectPreparationStates.Remarks, "Есть замечания"),
                new WorkspaceTextOption(WorkObjectPreparationStates.Ready, "Готов"),
                new WorkspaceTextOption(WorkObjectPreparationStates.Completed, "Завершено")
            ],
            [
                new WorkspaceTextOption("notReady", "Не готов"),
                new WorkspaceTextOption(
                    "readyWithConditions",
                    "Готов с условиями"),
                new WorkspaceTextOption("ready", "Готов")
            ]);
    }

    private static string GetAwpCaption(Awp awp)
        => !string.IsNullOrWhiteSpace(awp.Name)
            ? awp.Name
            : $"{awp.DateBegin:dd.MM.yyyy} — {awp.DateEnd:dd.MM.yyyy}";

}
