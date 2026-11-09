namespace Tmobis.Business.Process.Documents;

/// <summary>
/// Выбор последнего сформированного листа объекта в ОЗП с переходом к черновику при его отсутствии.
/// </summary>
public static class EvaluationSheetSelection
{
    public static Dictionary<Guid, EvaluationSheet> GetLatestByObject(IEnumerable<EvaluationSheet> sheets,
        Func<EvaluationSheet, bool>? isFormed = null)
    {
        isFormed ??= sheet => IsFormed(sheet.Requirements);
        return sheets.GroupBy(sheet => sheet.WorkObject.Id)
            .ToDictionary(group => group.Key, group => group
                .OrderByDescending(isFormed)
                .ThenByDescending(sheet => sheet.RegDate)
                .ThenByDescending(sheet => sheet.Id).First());
    }

    public static bool IsFormed(IEnumerable<EvaluationSheetRequirement> requirements)
    {
        var progress = EvaluationSheetProgress.Calculate(requirements);
        return progress.Total > 0 && progress.Filled == progress.Total;
    }

    public static Dictionary<Guid, EvaluationSheet> GetCurrentByObject(IEnumerable<EvaluationSheet> sheets)
        => sheets.GroupBy(sheet => sheet.WorkObject.Id)
            .ToDictionary(group => group.Key,
                group => group.OrderByDescending(sheet => sheet.RegDate)
                    .ThenByDescending(sheet => sheet.Id).First());
}
