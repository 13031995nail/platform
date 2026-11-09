namespace Tmobis.Business.Process.Documents;

using Tmobis.Core.Repositories;

/// <summary>
/// Загрузка признаков заполнения только для объектов с несколькими листами одного ОЗП.
/// </summary>
public sealed class EvaluationSheetSelectionProvider(IRepository repository)
{
    public async Task<Dictionary<Guid, EvaluationSheet>> GetLatestByObject(
        IReadOnlyCollection<EvaluationSheet> sheets)
    {
        var candidateIds = sheets.GroupBy(x => x.WorkObject.Id)
            .Where(group => group.Skip(1).Any()).SelectMany(group => group.Select(x => x.Id));
        var requirements = new List<EvaluationSheetRequirement>();
        foreach (var batch in candidateIds.Chunk(500))
        {
            var ids = batch.Cast<object>().ToList();
            requirements.AddRange(await repository.Find<EvaluationSheetRequirement>(
                requirement => ids.Contains(requirement.EvaluationSheet.Id),
                view: EvaluationSheetRequirement.Views.EvaluationSheetRequirementCompletionEntityView));
        }
        var bySheet = requirements.ToLookup(x => x.EvaluationSheet.Id);
        return EvaluationSheetSelection.GetLatestByObject(sheets,
            sheet => EvaluationSheetSelection.IsFormed(bySheet[sheet.Id]));
    }
}
