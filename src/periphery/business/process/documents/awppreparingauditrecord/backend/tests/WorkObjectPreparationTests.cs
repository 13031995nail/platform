namespace Tmobis.Business.Process.Documents.Tests;

using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Business.Process.Documents.Flexberry;
using Xunit;

public sealed class WorkObjectPreparationTests
{
    [Fact]
    public void NegativeSignedActDoesNotMeanAcceptanceOrAnUntouchedObject()
    {
        var result = WorkObjectPreparationCalculator.Create(Guid.NewGuid(), null,
            LevelReadiness.NotReady, false);
        Assert.Equal(WorkObjectPreparationStates.InProgress, result.State);
        Assert.Equal(LevelReadiness.NotReady, result.ReadinessLevel);
    }

    [Fact]
    public void NegativeSignedActDoesNotHideActiveReviewOrRemarks()
    {
        var sheet = CreateSheet();
        sheet.ReviewStartedAt = DateTime.UtcNow;
        Assert.Equal(WorkObjectPreparationStates.UnderReview,
            WorkObjectPreparationCalculator.Create(sheet.WorkObject?.Id ?? Guid.NewGuid(), sheet,
                LevelReadiness.NotReady, false).State);
        sheet.Requirements[0].ProblemComment = "Исправить";
        Assert.Equal(WorkObjectPreparationStates.Remarks,
            WorkObjectPreparationCalculator.Create(Guid.NewGuid(), sheet, LevelReadiness.NotReady, false).State);
    }

    [Fact]
    public void CardActionsFollowReviewTransitionsAndKeepBothDates()
    {
        var sheet = CreateSheet();
        var initial = Calculate(sheet);
        Assert.True(initial.CanSubmitForReview);
        Assert.False(initial.CanStartReview);
        Assert.False(initial.CanAccept);
        sheet.ReviewRequestedAt = DateTime.UtcNow.AddDays(-2);
        var submitted = Calculate(sheet);
        Assert.False(submitted.CanSubmitForReview);
        Assert.True(submitted.CanStartReview);
        Assert.False(submitted.CanAccept);
        sheet.ReviewStartedAt = DateTime.UtcNow;
        var started = Calculate(sheet);
        Assert.False(started.CanStartReview);
        Assert.True(started.CanAccept);
        Assert.Equal(sheet.ReviewRequestedAt, started.ReviewRequestedAt);
        Assert.Equal(sheet.ReviewStartedAt, started.ReviewStartedAt);
        sheet.Requirements[0].SupportingDocFile = "file.pdf";
        sheet.Requirements[0].IsVerified = true;
        sheet.IsVerifiedTSO = true;
        var accepted = Calculate(sheet);
        Assert.False(accepted.CanSubmitForReview);
        Assert.False(accepted.CanStartReview);
        Assert.False(accepted.CanAccept);
    }

    [Fact]
    public void IndependentlyStartedReviewIsVisibleWithoutSubmission()
    {
        var sheet = CreateSheet();
        sheet.ReviewStartedAt = DateTime.UtcNow;
        Assert.Equal(WorkObjectPreparationStates.UnderReview, Calculate(sheet).State);
        Assert.False(Calculate(sheet).CanSubmitForReview);
        Assert.True(Calculate(sheet).CanAccept);
    }

    [Fact]
    public void DefaultTsoFlagDoesNotMeanThatTheResultWasAccepted()
    {
        var sheet = CreateSheet();
        sheet.IsVerifiedTSO = true;
        Assert.Equal(WorkObjectPreparationStates.InProgress, Calculate(sheet).State);
    }

    [Fact]
    public void SubmissionDoesNotChangeTheDocumentStatus()
    {
        var sheet = CreateSheet();
        sheet.ReviewRequestedAt = DateTime.UtcNow;
        Assert.Equal(WorkObjectPreparationStates.UnderReview, Calculate(sheet).State);
        Assert.Null(sheet.ActStatus);
    }

    [Fact]
    public void UnresolvedRemarksTakePriorityOverAcceptance()
    {
        var sheet = CreateSheet();
        sheet.IsVerifiedTSO = true;
        sheet.Requirements[0].IsVerified = true;
        sheet.Requirements[0].ProblemComment = "Исправить";
        sheet.Requirements[0].Deadline = DateTime.Today.AddDays(-1);
        Assert.Equal(WorkObjectPreparationStates.Remarks, Calculate(sheet).State);
        Assert.True(Calculate(sheet).HasOverdueRemarks);
        sheet.Requirements[0].IsCompletedReq = true;
        Assert.Equal(WorkObjectPreparationStates.Ready, Calculate(sheet).State);
        Assert.False(Calculate(sheet).HasOverdueRemarks);
    }

    [Fact]
    public void ExistingSignedActWorksWithoutAConsumerSheet()
    {
        var result = WorkObjectPreparationCalculator.Create(Guid.NewGuid(), null,
            LevelReadiness.ReadyWithConditions, false);
        Assert.Equal(WorkObjectPreparationStates.Ready, result.State);
        Assert.Equal(LevelReadiness.ReadyWithConditions, result.ReadinessLevel);
    }

    [Fact]
    public void PassportCompletesTheProcessWithoutMandatoryRoleScreens()
        => Assert.Equal(WorkObjectPreparationStates.Completed,
            WorkObjectPreparationCalculator.Create(Guid.NewGuid(), null, null, true).State);

    [Fact]
    public void ProgressDoesNotModifyTheReadinessIndex()
    {
        var sheet = CreateSheet();
        sheet.ObjectReadinessIndex = 0.834m;
        sheet.Requirements[0].SupportingDocFile = "file.pdf";
        Assert.Equal(100m, Calculate(sheet).Progress!.Percent);
        Assert.Equal(0.834m, sheet.ObjectReadinessIndex);
    }

    [Fact]
    public void SignedSheetDoesNotExposeReviewChanges()
    {
        var sheet = CreateSheet();
        Assert.True(Calculate(sheet).CanChangeEvaluationSheet);
        sheet.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };
        Assert.False(Calculate(sheet).CanChangeEvaluationSheet);
        Assert.False(Calculate(sheet).CanSubmitForReview);
        Assert.False(Calculate(sheet).CanStartReview);
        Assert.False(Calculate(sheet).CanAccept);
        sheet.ActStatus.Id = IActStatus.Keys.Approved;
        Assert.False(Calculate(sheet).CanChangeEvaluationSheet);
    }

    private static WorkObjectPreparationInfo Calculate(EvaluationSheet sheet)
        => WorkObjectPreparationCalculator.Create(Guid.NewGuid(), sheet, null, false);

    private static EvaluationSheet CreateSheet()
    {
        var sheet = new EvaluationSheet();
        sheet.Requirements.Add(new EvaluationSheetRequirement
        {
            EvaluationSheet = sheet, ReqOrder = 2, SupportingDocName = "План"
        });
        return sheet;
    }
}
