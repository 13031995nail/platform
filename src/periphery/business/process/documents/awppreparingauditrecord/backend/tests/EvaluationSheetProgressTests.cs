namespace Tmobis.Business.Process.Documents.Tests;

using Xunit;

public sealed class EvaluationSheetProgressTests
{
    [Fact]
    public void EnteredZeroDiffersFromAnEmptyNumericValue()
    {
        var requirement = new EvaluationSheetRequirement
        {
            ReqOrder = 43, IsCalculated = true, IsOptional = true, CalculatedReq = 0
        };
        Assert.False(EvaluationSheetProgress.IsFilled(requirement));
        requirement.HasEnteredValue = true;
        Assert.True(EvaluationSheetProgress.IsFilled(requirement));
        requirement.HasEnteredValue = false;
        Assert.False(EvaluationSheetProgress.IsFilled(requirement));
    }

    [Fact]
    public void RequiredDocumentMustBeAttachedRegardlessOfReadinessValue()
    {
        var requirement = new EvaluationSheetRequirement
        {
            ReqOrder = 2, SupportingDocName = "План", CalculatedReq = 1
        };
        Assert.True(EvaluationSheetProgress.HasMissingDocument(requirement));
        Assert.False(EvaluationSheetProgress.IsFilled(requirement));
        requirement.SupportingDocFile = "document.pdf";
        Assert.True(EvaluationSheetProgress.IsFilled(requirement));
    }

    [Fact]
    public void AggregateRowsOptionalDocumentsAndSeparatorsDoNotIncreaseDenominator()
    {
        var requirements = new[]
        {
            new EvaluationSheetRequirement { ReqOrder = 1, IsCalculated = true },
            new EvaluationSheetRequirement { ReqOrder = 0 },
            new EvaluationSheetRequirement { ReqOrder = 36, IsOptional = true, SupportingDocName = "Документ" },
            new EvaluationSheetRequirement { ReqOrder = 2, SupportingDocName = "План", SupportingDocFile = "plan.pdf" },
            new EvaluationSheetRequirement { ReqOrder = 43, IsCalculated = true, IsOptional = true }
        };
        var progress = EvaluationSheetProgress.Calculate(requirements);
        Assert.Equal(2, progress.Total);
        Assert.Equal(1, progress.Filled);
        Assert.Equal(50m, progress.Percent);
    }

    [Fact]
    public void EmptyDenominatorDoesNotPretendToBeCompleted()
        => Assert.Null(EvaluationSheetProgress.Calculate([]).Percent);
}
