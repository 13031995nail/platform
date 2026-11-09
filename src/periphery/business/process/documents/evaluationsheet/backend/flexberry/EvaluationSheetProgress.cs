namespace Tmobis.Business.Process.Documents;

/// <summary>
/// Вычисление заполненности существующих требований без изменения индекса готовности.
/// </summary>
public static class EvaluationSheetProgress
{
    public static bool IsCounted(EvaluationSheetRequirement requirement)
        => requirement.ReqOrder != 0
            && (requirement.IsCalculated && requirement.IsOptional
                || !requirement.IsCalculated && !requirement.IsOptional
                    && !string.IsNullOrWhiteSpace(requirement.SupportingDocName));

    public static bool HasMissingDocument(EvaluationSheetRequirement requirement)
        => IsCounted(requirement)
            && !requirement.IsCalculated
            && !requirement.IsOptional
            && !string.IsNullOrWhiteSpace(requirement.SupportingDocName)
            && string.IsNullOrWhiteSpace(requirement.SupportingDocFile);

    public static bool IsFilled(EvaluationSheetRequirement requirement)
        => IsCounted(requirement)
            && !HasMissingDocument(requirement)
            && (!requirement.IsCalculated || requirement.HasEnteredValue);

    public static RequirementProgress Calculate(IEnumerable<EvaluationSheetRequirement> requirements)
    {
        var counted = requirements.Where(IsCounted).ToList();
        var filled = counted.Count(IsFilled);
        return new RequirementProgress(counted.Count, filled,
            counted.Count == 0 ? null : Math.Round(100m * filled / counted.Count, 1));
    }
}

public sealed record RequirementProgress(int Total, int Filled, decimal? Percent);
