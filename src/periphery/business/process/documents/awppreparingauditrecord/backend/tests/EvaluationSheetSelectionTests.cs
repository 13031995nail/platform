namespace Tmobis.Business.Process.Documents.Tests;

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Core.Repositories;
using Xunit;

public sealed class EvaluationSheetSelectionTests
{
    [Fact]
    public async Task ActUsesTheSameFormedSheetAsTheCardAndIgnoresOtherPeriods()
    {
        var formed = CreateSheet(1, true);
        formed.ObjectReadinessIndex = 0.85m;
        formed.ObjectReadinessLevel = LevelReadiness.ReadyWithConditions;
        var draft = CreateSheet(2, false, formed.WorkObject, formed.Awp);
        draft.ObjectReadinessIndex = 0.1m;
        var otherPeriod = CreateSheet(3, true, formed.WorkObject, new Awp());
        otherPeriod.ObjectReadinessIndex = 1m;
        var fixture = new Fixture([draft, otherPeriod, formed]);
        var act = new RedinessAct { Organization = formed.WorkObject.Organization, Awp = formed.Awp };
        var builder = new ActBuilder(fixture.Repository, fixture.Selection);
        var card = Assert.Single(await fixture.Preparation.GetObjectPreparationInfo(formed.Awp.Id, [formed.WorkObject.Id]));
        var detail = Assert.Single(await builder.BuildObjects(act));
        Assert.Equal(card.ReadinessLevel, detail.ObjectReadiness);
        Assert.Equal(card.ReadinessIndex, act.AvgObjectReadinessIndex);
        draft.Requirements[0].SupportingDocFile = "новый.pdf";
        draft.ObjectReadinessLevel = LevelReadiness.NotReady;
        detail = Assert.Single(await builder.BuildObjects(act));
        Assert.Equal(LevelReadiness.NotReady, detail.ObjectReadiness);
        Assert.Equal(draft.ObjectReadinessIndex, act.AvgObjectReadinessIndex);
    }

    [Fact]
    public void IncompleteReplacementDoesNotHidePreviouslyFormedSheet()
    {
        var old = CreateSheet(1, true);
        var draft = CreateSheet(2, false, old.WorkObject, old.Awp);
        Assert.Same(old, EvaluationSheetSelection.GetLatestByObject([old, draft])[old.WorkObject.Id]);
        Assert.Same(draft, EvaluationSheetSelection.GetCurrentByObject([old, draft])[old.WorkObject.Id]);
        Assert.False(old.IsVerifiedTSO);
        Assert.False(old.Requirements[0].IsVerified);
        Assert.Null(old.ActStatus);
    }

    [Fact]
    public void NewestDraftIsUsedUntilAnySheetIsFormed()
    {
        var old = CreateSheet(1, false);
        var draft = CreateSheet(2, false, old.WorkObject, old.Awp);
        Assert.Same(draft, EvaluationSheetSelection.GetLatestByObject([old, draft])[old.WorkObject.Id]);
    }

    [Fact]
    public void CompletedReplacementBecomesCurrentWithoutReviewOrSignature()
    {
        var old = CreateSheet(1, true);
        var current = CreateSheet(2, true, old.WorkObject, old.Awp);
        old.ChangeStatusDate = DateTime.UtcNow.AddDays(1);
        Assert.Same(current, EvaluationSheetSelection.GetLatestByObject([old, current])[old.WorkObject.Id]);
    }

    [Fact]
    public void EmptySheetIsNotFormed()
        => Assert.False(EvaluationSheetSelection.IsFormed([]));

    [Fact]
    public void NumericZeroIsFilledWhenItWasExplicitlyEntered()
    {
        var requirement = new EvaluationSheetRequirement
        {
            ReqOrder = 6, IsCalculated = true, IsOptional = true,
            HasEnteredValue = true, CalculatedReq = 0
        };
        Assert.True(EvaluationSheetSelection.IsFormed([requirement]));
        requirement.HasEnteredValue = false;
        Assert.False(EvaluationSheetSelection.IsFormed([requirement]));
    }

    [Fact]
    public void UncountedRequirementsAndRemarksDoNotRequireReviewerApprovalToFormSheet()
    {
        var sheet = CreateSheet(1, true);
        sheet.Requirements[0].ProblemComment = "Исправить";
        sheet.Requirements.Add(new EvaluationSheetRequirement { EvaluationSheet = sheet, ReqOrder = 0 });
        Assert.True(EvaluationSheetSelection.IsFormed(sheet.Requirements));
        sheet.Requirements[0].SupportingDocFile = null;
        Assert.False(EvaluationSheetSelection.IsFormed(sheet.Requirements));
    }

    [Fact]
    public async Task SingleSheetDoesNotNeedExtraCompletionQuery()
    {
        var fixture = new Fixture([CreateSheet(1, false)]);
        var selected = await fixture.Selection.GetLatestByObject(fixture.Sheets);
        Assert.Single(selected);
        Assert.Equal(0, fixture.RequirementReads);
    }

    [Fact]
    public async Task CompletionQueryDoesNotIncludeObjectsWithoutMultipleSheets()
    {
        var formed = CreateSheet(1, true);
        var draft = CreateSheet(2, false, formed.WorkObject, formed.Awp);
        var single = CreateSheet(3, false);
        var fixture = new Fixture([formed, draft, single]);
        var selected = await fixture.Selection.GetLatestByObject(fixture.Sheets);
        Assert.Same(formed, selected[formed.WorkObject.Id]);
        Assert.Same(single, selected[single.WorkObject.Id]);
        Assert.Equal(1, fixture.RequirementReads);
        Assert.Equal(2, fixture.RequirementBatchSizes.Single());
    }

    [Fact]
    public async Task CompletionQueriesAreBatchedInsteadOfLoadingRequirementsPerSheet()
    {
        var sheets = Enumerable.Range(0, 251).SelectMany(_ =>
        {
            var formed = CreateSheet(1, true);
            return new[] { formed, CreateSheet(2, false, formed.WorkObject, formed.Awp) };
        }).ToList();
        var fixture = new Fixture(sheets);
        var selected = await fixture.Selection.GetLatestByObject(sheets);
        Assert.Equal(251, selected.Count);
        Assert.Equal(2, fixture.RequirementReads);
        Assert.All(fixture.RequirementBatchSizes, size => Assert.InRange(size, 1, 500));
    }

    [Fact]
    public async Task PreparationUsesFormedSheetButKeepsLinkToCurrentDraft()
    {
        var formed = CreateSheet(1, true);
        var draft = CreateSheet(2, false, formed.WorkObject, formed.Awp);
        var fixture = new Fixture([formed, draft]);
        var result = Assert.Single(await fixture.Preparation.GetObjectPreparationInfo(formed.Awp.Id, [formed.WorkObject.Id]));
        Assert.Equal(formed.Id, result.EvaluationSheetId);
        Assert.Equal(draft.Id, result.DraftEvaluationSheetId);
        Assert.Equal(100m, result.Progress!.Percent);
        draft.Requirements[0].SupportingDocFile = "новый.pdf";
        var replaced = Assert.Single(await fixture.Preparation.GetObjectPreparationInfo(formed.Awp.Id, [formed.WorkObject.Id]));
        Assert.Equal(draft.Id, replaced.EvaluationSheetId);
        Assert.Null(replaced.DraftEvaluationSheetId);
    }

    [Fact]
    public async Task CompletedSheetFromAnotherAwpDoesNotHideCurrentDraft()
    {
        var old = CreateSheet(1, true);
        var current = CreateSheet(2, false, old.WorkObject, new Awp());
        var fixture = new Fixture([old, current]);
        var result = Assert.Single(await fixture.Preparation.GetObjectPreparationInfo(current.Awp.Id, [current.WorkObject.Id]));
        Assert.Equal(current.Id, result.EvaluationSheetId);
        Assert.Null(result.DraftEvaluationSheetId);
        Assert.Equal(0m, result.Progress!.Percent);
    }

    private static EvaluationSheet CreateSheet(int day, bool filled, WorkObject? workObject = null, Awp? awp = null)
    {
        var sheet = new EvaluationSheet
        {
            WorkObject = workObject ?? new WorkObject
            {
                Organization = new Tmobis.Business.Common.Contacts.Organization(), Actuality = true
            }, Awp = awp ?? new Awp(),
            RegDate = new DateTime(2026, 9, day)
        };
        sheet.Organization = sheet.WorkObject.Organization;
        sheet.Requirements.Add(new EvaluationSheetRequirement
        {
            EvaluationSheet = sheet, ReqOrder = 2, SupportingDocName = "План",
            SupportingDocFile = filled ? "план.pdf" : null
        });
        return sheet;
    }

    private sealed class Fixture(List<EvaluationSheet> sheets)
    {
        public List<EvaluationSheet> Sheets { get; } = sheets;
        public int RequirementReads { get; private set; }
        public List<int> RequirementBatchSizes { get; } = [];
        private IRepository? repository;
        public IRepository Repository
        {
            get
            {
                if (repository != null) return repository;
                repository = DispatchProxy.Create<IRepository, RepositoryProxy>();
                ((RepositoryProxy)repository).Handler = (method, args) => method.Name switch
                {
                    "Query" when method.GetGenericArguments()[0] == typeof(WorkObject)
                        => Sheets.Select(x => x.WorkObject).DistinctBy(x => x.Id).AsQueryable(),
                    "MakeEntity" when method.GetGenericArguments()[0] == typeof(RedinessActObject)
                        => new RedinessActObject(),
                    "Find" when method.GetGenericArguments()[0] == typeof(EvaluationSheet)
                        => Task.FromResult(Sheets.Where(((Expression<Func<EvaluationSheet, bool>>)args![0]!).Compile()).ToList()),
                    "Find" when method.GetGenericArguments()[0] == typeof(EvaluationSheetRequirement)
                        => ReadRequirements((Expression<Func<EvaluationSheetRequirement, bool>>)args![0]!),
                    "Find" when method.GetGenericArguments()[0] == typeof(RedinessActObject)
                        => Task.FromResult(new List<RedinessActObject>()),
                    "Find" when method.GetGenericArguments()[0] == typeof(AwpReadinessPassportObject)
                        => Task.FromResult(new List<AwpReadinessPassportObject>()),
                    _ => throw new NotSupportedException(method.Name)
                };
                return repository;
            }
        }
        public EvaluationSheetSelectionProvider Selection => new(Repository);
        public AwpPreparingAuditRecordProvider Preparation
            => new(Repository, NullLogger<AwpPreparingAuditRecordProvider>.Instance, Selection);

        private Task<List<EvaluationSheetRequirement>> ReadRequirements(Expression<Func<EvaluationSheetRequirement, bool>> predicate)
        {
            RequirementReads++;
            var result = Sheets.SelectMany(x => x.Requirements).Where(predicate.Compile()).ToList();
            RequirementBatchSizes.Add(result.Count);
            return Task.FromResult(result);
        }
    }

    private sealed class ActBuilder(IRepository repository, EvaluationSheetSelectionProvider selection)
        : OnRedinessActCreated(repository, selection)
    {
        public Task<List<RedinessActObject>> BuildObjects(RedinessAct act) => CreateObjects(act);
    }

    public class RepositoryProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler(targetMethod!, args);
    }
}
