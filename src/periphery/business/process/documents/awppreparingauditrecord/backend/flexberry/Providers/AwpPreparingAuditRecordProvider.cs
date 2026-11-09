namespace Tmobis.Business.Process.Documents.Providers;

using Microsoft.Extensions.Logging;
using Tmobis.Core.Repositories;

/// <summary>
/// Методы ЖК.
/// </summary>
public class AwpPreparingAuditRecordProvider(
    IRepository repository,
    ILogger<AwpPreparingAuditRecordProvider> logger,
    EvaluationSheetSelectionProvider selectionProvider)
: IAwpPreparingAuditRecordProvider
{
    private const int QueryBatchSize = 500;

    public async Task UpdatePartnerRecords(Guid awpId, Guid organizationId)
    {
        try
        {
            // вычислить общий индекс по организации
            var awpRecords = await repository.Find<AwpPreparingAuditRecord>(x => x.Awp.Id == awpId && x.WorkObject.Organization.Id == organizationId);
            if (awpRecords.Count == 0)
            {
                return;
            }
            var averageIndex = awpRecords.Average(x => x.IndexReadiness.HasValue ? x.IndexReadiness.Value : 0);
            var partnerReadiness = PartnerReadinessLevel.Empty;
            if (averageIndex < 0.8m)
            {
                partnerReadiness = PartnerReadinessLevel.NotReady;
            }
            else if (averageIndex >= 0.8m && averageIndex < 0.9m)
            {
                partnerReadiness = PartnerReadinessLevel.ConditionReady;
            }
            else if (averageIndex >= 0.9m)
            {
                partnerReadiness = PartnerReadinessLevel.Ready;
            }
            awpRecords.ForEach(r =>
            {
                r.PartnerReadiness = partnerReadiness;
                r.IndexPartnerReadiness = averageIndex;
                repository.Update(r);
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Произошла ошибка в методе UpdatePartnerRecords");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkObjectPreparationInfo>> GetObjectPreparationInfo(
        Guid? awpId,
        IReadOnlyCollection<Guid> workObjectIds)
    {
        ArgumentNullException.ThrowIfNull(workObjectIds);

        var objectIds = workObjectIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
        if (objectIds.Count == 0)
        {
            return [];
        }

        if (!awpId.HasValue)
        {
            return objectIds.Select(id => WorkObjectPreparationCalculator.Create(id, null, null, false)).ToList();
        }

        var currentAwpId = awpId.Value;
        var sheets = new List<EvaluationSheet>();
        var readinessObjects = new List<RedinessActObject>();
        var passportObjects = new List<AwpReadinessPassportObject>();
        foreach (var objectIdBatch in objectIds.Chunk(QueryBatchSize))
        {
            var objectIdValues = objectIdBatch.Cast<object>().ToList();
            sheets.AddRange(await repository.Find<EvaluationSheet>(
                sheet => sheet.Awp.Id == currentAwpId
                    && objectIdValues.Contains(sheet.WorkObject.Id),
                view: EvaluationSheet.Views.EvaluationSheetPreparationEntityView));
            readinessObjects.AddRange(await repository.Find<RedinessActObject>(
                readinessObject => readinessObject.Act != null
                    && readinessObject.Act.Awp.Id == currentAwpId
                    && readinessObject.Act.ActStatus.Id == IActStatus.Keys.Signed
                    && readinessObject.WorkObject != null
                    && objectIdValues.Contains(readinessObject.WorkObject.Id),
                view: RedinessActObject.Views.RedinessActObjectDEntityView));
            passportObjects.AddRange(await repository.Find<AwpReadinessPassportObject>(
                passportObject => passportObject.Act != null
                    && passportObject.Act.Awp != null
                    && passportObject.Act.Awp.Id == currentAwpId
                    && passportObject.Act.ActStatus.Id == IActStatus.Keys.Signed
                    && passportObject.WorkObject != null
                    && objectIdValues.Contains(passportObject.WorkObject.Id),
                view: AwpReadinessPassportObject.Views.AwpReadinessPassportObjectEEntityView));
        }

        var latestSheets = await selectionProvider.GetLatestByObject(sheets);
        var currentSheets = EvaluationSheetSelection.GetCurrentByObject(sheets);
        var latestSheetIds = latestSheets.Values
            .Select(sheet => sheet.Id)
            .ToList();
        var requirements = new List<EvaluationSheetRequirement>();
        foreach (var sheetIdBatch in latestSheetIds.Chunk(QueryBatchSize))
        {
            var sheetIdValues = sheetIdBatch.Cast<object>().ToList();
            requirements.AddRange(await repository.Find<EvaluationSheetRequirement>(
                requirement => sheetIdValues.Contains(requirement.EvaluationSheet.Id),
                view: EvaluationSheetRequirement.Views.EvaluationSheetRequirementWorkspaceEntityView));
        }
        var requirementsBySheet = requirements
            .GroupBy(requirement => requirement.EvaluationSheet.Id)
            .ToDictionary(group => group.Key, group => group.ToList());
        var readinessByObject = readinessObjects
            .Where(readinessObject => readinessObject.WorkObject != null)
            .GroupBy(readinessObject => readinessObject.WorkObject!.Id)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending(x => x.Act!.ChangeStatusDate ?? x.Act.RegDate)
                    .ThenByDescending(x => x.Act!.Id)
                    .First().ObjectReadiness);
        var completedObjectIds = passportObjects
            .Where(passportObject => passportObject.WorkObject != null)
            .Select(passportObject => passportObject.WorkObject!.Id)
            .ToHashSet();

        return objectIds.Select(objectId =>
        {
            var sheet = latestSheets.GetValueOrDefault(objectId);
            var sheetRequirements = sheet != null
                && requirementsBySheet.TryGetValue(sheet.Id, out var loadedRequirements)
                    ? loadedRequirements
                    : [];
            return WorkObjectPreparationCalculator.Create(
                objectId,
                sheet,
                sheetRequirements,
                readinessByObject.TryGetValue(objectId, out var readiness) ? readiness : null,
                completedObjectIds.Contains(objectId)) with
            {
                DraftEvaluationSheetId = currentSheets.TryGetValue(objectId, out var current)
                    && current.Id != sheet?.Id ? current.Id : null
            };
        }).ToList();
    }

}
