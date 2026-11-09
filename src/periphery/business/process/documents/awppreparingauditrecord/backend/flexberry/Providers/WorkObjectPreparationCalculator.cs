namespace Tmobis.Business.Process.Documents.Providers;

/// <summary>
/// Вычисление состояния по результатам работы, независимо от способа их получения.
/// </summary>
public static class WorkObjectPreparationCalculator
{
    public static WorkObjectPreparationInfo Create(Guid objectId, EvaluationSheet? sheet,
        LevelReadiness? actReadiness, bool hasPassport)
    {
        var requirements = sheet?.Requirements.Cast<EvaluationSheetRequirement>().ToList() ?? [];
        return Create(objectId, sheet, requirements, actReadiness, hasPassport);
    }

    public static WorkObjectPreparationInfo Create(Guid objectId, EvaluationSheet? sheet,
        IReadOnlyCollection<EvaluationSheetRequirement> requirements,
        LevelReadiness? actReadiness, bool hasPassport)
    {
        var hasRemarks = requirements.Any(HasOpenRemark);
        var progress = EvaluationSheetProgress.Calculate(requirements);
        var accepted = EvaluationSheetReviewService.IsAccepted(sheet, requirements);
        var state = hasPassport ? WorkObjectPreparationStates.Completed
            : hasRemarks ? WorkObjectPreparationStates.Remarks
            : actReadiness is LevelReadiness.Ready or LevelReadiness.ReadyWithConditions || accepted
                ? WorkObjectPreparationStates.Ready
            : IsUnderReview(sheet) ? WorkObjectPreparationStates.UnderReview
            : sheet == null && !actReadiness.HasValue ? WorkObjectPreparationStates.NotStarted
            : WorkObjectPreparationStates.InProgress;

        return new WorkObjectPreparationInfo(objectId, state,
            actReadiness ?? sheet?.ObjectReadinessLevel ?? LevelReadiness.NotReady,
            sheet == null || requirements.Any(EvaluationSheetProgress.HasMissingDocument),
            requirements.Any(x => HasOpenRemark(x) && x.Deadline?.Date < DateTime.Today),
            progress, sheet?.Id, sheet?.ObjectReadinessIndex)
        {
            CanChangeEvaluationSheet = EvaluationSheetReviewService.CanChangeReview(sheet),
            CanSubmitForReview = EvaluationSheetReviewService.CanSubmit(sheet, requirements),
            CanStartReview = EvaluationSheetReviewService.CanStartReview(sheet, requirements),
            CanAccept = EvaluationSheetReviewService.CanAccept(sheet, requirements),
            ReviewRequestedAt = sheet?.ReviewRequestedAt,
            ReviewStartedAt = sheet?.ReviewStartedAt
        };
    }

    private static bool HasOpenRemark(EvaluationSheetRequirement requirement)
        => !string.IsNullOrWhiteSpace(requirement.ProblemComment) && !requirement.IsCompletedReq;

    private static bool IsUnderReview(EvaluationSheet? sheet)
        => sheet?.ReviewRequestedAt != null
            || sheet?.ReviewStartedAt != null
            || sheet?.ActStatus?.Id is var statusId
                && (statusId == IActStatus.Keys.Signing || statusId == IActStatus.Keys.Signed);
}
