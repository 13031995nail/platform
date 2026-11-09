namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Process.Documents.Models;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;

public sealed class WorkObjectRemarksProvider(
    IRepository repository,
    WorkspaceObjectScopeProvider scopeProvider,
    IAwpPreparingAuditRecordProvider preparationProvider,
    WorkObjectRequirementMapper requirementMapper,
    EvaluationSheetPermissionProvider permissionProvider,
    EvaluationSheetSelectionProvider selectionProvider)
{
    private const int QueryBatchSize = 500;

    public async Task<IReadOnlyList<WorkObjectRemarkGroup>> GetRemarks(WorkspaceDashboardQuery query)
    {
        if (query.Workspace?.Trim().ToLowerInvariant() is not ("tso" or "administration")
            || !(await permissionProvider.GetPermissions()).CanReview)
        {
            throw new UnauthorizedAccessException();
        }
        var scope = await scopeProvider.GetScope(query, includeCommission: true);
        var workObjects = WorkspaceQueryFilters.ApplyObjectFilters(scope.Objects, query);
        if (scope.Awp == null || workObjects.Count == 0)
        {
            return [];
        }
        if (!string.IsNullOrWhiteSpace(query.State) || !string.IsNullOrWhiteSpace(query.ReadinessLevel))
        {
            var preparation = await preparationProvider.GetObjectPreparationInfo(scope.Awp.Id,
                workObjects.Select(x => x.Id).ToList());
            var filteredIds = WorkspaceQueryFilters.ApplyStateFilters(preparation, query)
                .Select(x => x.WorkObjectId).ToHashSet();
            workObjects = workObjects.Where(x => filteredIds.Contains(x.Id)).ToList();
        }
        var objectIds = workObjects.Select(x => x.Id).ToList();
        var objects = workObjects.ToDictionary(x => x.Id);
        var sheets = new List<EvaluationSheet>();
        foreach (var objectIdBatch in objectIds.Chunk(QueryBatchSize))
        {
            var objectIdValues = objectIdBatch.Cast<object>().ToList();
            sheets.AddRange(await repository.Find<EvaluationSheet>(
                x => x.Awp.Id == scope.Awp.Id
                    && objectIdValues.Contains(x.WorkObject.Id),
                view: EvaluationSheet.Views.EvaluationSheetPreparationEntityView));
        }
        sheets = (await selectionProvider.GetLatestByObject(sheets)).Values.ToList();
        var requirements = new List<EvaluationSheetRequirement>();
        foreach (var sheetIdBatch in sheets.Select(x => x.Id).Chunk(QueryBatchSize))
        {
            var sheetIdValues = sheetIdBatch.Cast<object>().ToList();
            requirements.AddRange(await repository.Find<EvaluationSheetRequirement>(
                x => sheetIdValues.Contains(x.EvaluationSheet.Id)
                    && x.ProblemComment != null
                    && x.ProblemComment != string.Empty,
                view: EvaluationSheetRequirement.Views.EvaluationSheetRequirementWorkspaceEntityView));
        }
        requirements = requirements
            .Where(x => !string.IsNullOrWhiteSpace(x.ProblemComment))
            .ToList();
        var mappedRequirements = (await requirementMapper.Map(requirements))
            .ToDictionary(x => x.Id);
        var requirementsBySheet = requirements
            .GroupBy(x => x.EvaluationSheet.Id)
            .ToDictionary(x => x.Key, x => x.ToList());
        var result = new List<WorkObjectRemarkGroup>();
        foreach (var sheet in sheets.OrderByDescending(x => x.RegDate)
            .Where(x => requirementsBySheet.ContainsKey(x.Id)))
        {
            var sheetRequirements = requirementsBySheet[sheet.Id]
                .OrderBy(x => x.IsCompletedReq)
                .ThenBy(x => x.Deadline)
                .Select(x => mappedRequirements[x.Id])
                .ToList();
            result.Add(new WorkObjectRemarkGroup(sheet.WorkObject.Id,
                objects[sheet.WorkObject.Id].Name, sheet.Id,
                sheetRequirements));
        }
        return result;
    }
}
