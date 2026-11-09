namespace Tmobis.Business.Process.Documents;

using Tmobis.Frameworks.Flexberry.Web.OData.Access;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using ICSSoft.STORMNET;

public sealed class EvaluationSheetAccess(
    IWorkObjectAccessProvider workObjectAccessProvider,
    ActSignAccessValidator actSignAccessValidator,
    IRepository repository,
    EvaluationSheetPermissionProvider permissionProvider,
    EvaluationSheetReviewService reviewService)
{
    public async Task EnsureCanChange(EvaluationSheet sheet)
    {
        var workObjectIds = await EnsureRelatedEntity(
            sheet.WorkObject?.Id,
            sheet.GetStatus() == ObjectStatus.Created ? Guid.Empty : sheet.Id,
            async () =>
            {
                var stored = await repository.GetById<EvaluationSheet>(sheet.Id,
                    EvaluationSheet.Views.EvaluationSheetPreparationEntityView);
                return (stored?.WorkObject?.Id, stored?.Awp?.Id);
            }, sheet.Awp?.Id);
        if (sheet.GetStatus() == ObjectStatus.Created ? sheet.IsVerifiedTSO
            : sheet.GetAlteredPropertyNames(true).Contains(nameof(EvaluationSheet.IsVerifiedTSO)))
        {
            await EnsureReviewer(workObjectIds,
                sheet.GetStatus() == ObjectStatus.Created ? null : sheet.Id, sheet.Awp?.Id);
        }
        var created = sheet.GetStatus() == ObjectStatus.Created;
        var changes = sheet.GetAlteredPropertyNames(true);
        if (created ? sheet.ReviewStartedAt.HasValue
            : changes.Contains(nameof(EvaluationSheet.ReviewStartedAt)))
        {
            var storedSheet = created ? sheet : await repository.GetById<EvaluationSheet>(sheet.Id,
                EvaluationSheet.Views.EvaluationSheetEEntityView);
            if (!EvaluationSheetReviewService.CanChangeReview(storedSheet))
            {
                throw new ArgumentException("Подписанный или утверждённый документ нельзя изменить.");
            }
            if (!created && !sheet.ReviewStartedAt.HasValue && sheet.ReviewRequestedAt.HasValue
                && changes.Contains(nameof(EvaluationSheet.ReviewRequestedAt))
                && storedSheet != null && EvaluationSheetReviewService.CanSubmit(storedSheet))
            {
                foreach (var workObjectId in workObjectIds)
                {
                    await reviewService.EnsureCanRequestReview(workObjectId, storedSheet.Awp?.Id, allowReviewer: false);
                }
            }
            else
            {
                await EnsureReviewer(workObjectIds, created ? null : sheet.Id, sheet.Awp?.Id);
            }
            if (sheet.ReviewStartedAt.HasValue)
            {
                sheet.ReviewStartedAt = storedSheet?.ReviewStartedAt ?? DateTime.UtcNow;
                if (created)
                {
                    sheet.ReviewStartedAt = DateTime.UtcNow;
                }
            }
        }
        if (sheet.GetStatus() == ObjectStatus.Created ? sheet.ReviewRequestedAt.HasValue
            : sheet.GetAlteredPropertyNames(true).Contains(nameof(EvaluationSheet.ReviewRequestedAt)))
        {
            var storedSheet = sheet.GetStatus() == ObjectStatus.Created ? sheet
                : await repository.GetById<EvaluationSheet>(sheet.Id,
                    EvaluationSheet.Views.EvaluationSheetPreparationEntityView);
            if (!EvaluationSheetReviewService.CanChangeReview(storedSheet))
            {
                throw new ArgumentException("Подписанный или утверждённый документ нельзя изменить.");
            }
            foreach (var workObjectId in workObjectIds)
            {
                await reviewService.EnsureCanRequestReview(workObjectId, storedSheet?.Awp?.Id ?? sheet.Awp?.Id);
                if (!sheet.ReviewRequestedAt.HasValue
                    && !(await permissionProvider.GetPermissions(workObjectId, storedSheet?.Awp?.Id ?? sheet.Awp?.Id)).CanReview)
                {
                    throw new System.UnauthorizedAccessException("Отменять проверку может только проверяющий.");
                }
            }
            if (sheet.ReviewRequestedAt.HasValue)
            {
                sheet.ReviewRequestedAt = DateTime.UtcNow;
            }
        }
    }

    public async Task EnsureCanChange(EvaluationSheetRequirement requirement)
    {
        var workObjectIds = await EnsureRelatedEntity(
            requirement.EvaluationSheet?.WorkObject?.Id,
            requirement.GetStatus() == ObjectStatus.Created ? Guid.Empty : requirement.Id,
            async () =>
            {
                var stored = await repository.GetById<EvaluationSheetRequirement>(requirement.Id,
                    EvaluationSheetRequirement.Views.EvaluationSheetRequirementAccessEntityView);
                return (stored?.EvaluationSheet?.WorkObject?.Id, stored?.EvaluationSheet?.Awp?.Id);
            }, requirement.EvaluationSheet?.Awp?.Id);
        if (requirement.GetStatus() == ObjectStatus.Created)
        {
            if (requirement.IsVerified || requirement.IsCompletedReq
                || !string.IsNullOrWhiteSpace(requirement.ProblemComment) || requirement.Deadline.HasValue)
            {
                await EnsureReviewer(workObjectIds,
                    requirement.EvaluationSheet?.GetStatus() == ObjectStatus.Created ? null : requirement.EvaluationSheet?.Id,
                    requirement.EvaluationSheet?.Awp?.Id);
            }
        }
        else
        {
            var changes = requirement.GetAlteredPropertyNames(true);
            if (changes.Intersect(new[]
                {
                    nameof(EvaluationSheetRequirement.IsVerified),
                    nameof(EvaluationSheetRequirement.IsCompletedReq),
                    nameof(EvaluationSheetRequirement.ProblemComment),
                    nameof(EvaluationSheetRequirement.Deadline)
                }).Any())
            {
                await EnsureReviewer(workObjectIds, requirement.EvaluationSheet?.Id, requirement.EvaluationSheet?.Awp?.Id);
            }
            if (changes.Intersect(new[]
                {
                    nameof(EvaluationSheetRequirement.IsCalculated),
                    nameof(EvaluationSheetRequirement.IsOptional),
                    nameof(EvaluationSheetRequirement.ReqOrder),
                    nameof(EvaluationSheetRequirement.ReqNumber),
                    nameof(EvaluationSheetRequirement.ShortReqName),
                    nameof(EvaluationSheetRequirement.IndicatorName),
                    nameof(EvaluationSheetRequirement.Weight),
                    nameof(EvaluationSheetRequirement.SortOrder),
                    nameof(EvaluationSheetRequirement.EvaluationSheet),
                    nameof(EvaluationSheetRequirement.VerificationRequirement),
                    nameof(EvaluationSheetRequirement.SupportingDocName)
                }).Any())
            {
                await workObjectAccessProvider.EnsureTechnicalAdministrator();
            }
        }
    }

    private async Task EnsureReviewer(IEnumerable<Guid> workObjectIds, Guid? sheetId, Guid? awpId = null)
    {
        var sheet = sheetId.HasValue
            ? await repository.GetById<EvaluationSheet>(sheetId.Value,
                EvaluationSheet.Views.EvaluationSheetPreparationEntityView)
            : null;
        var periods = new[] { sheet?.Awp?.Id, awpId }
            .Where(id => id.HasValue).Distinct().ToList();
        if (periods.Count == 0)
        {
            periods.Add(null);
        }
        foreach (var workObjectId in workObjectIds)
        {
            foreach (var periodId in periods)
            {
                if (!(await permissionProvider.GetPermissions(workObjectId, periodId)).CanReview)
                {
                    throw new System.UnauthorizedAccessException("Недостаточно полномочий для изменения результатов проверки.");
                }
            }
        }
    }

    public async Task EnsureCanChange(EvaluationSheetSign sign)
    {
        await EnsureRelatedEntity(
            sign.Act?.WorkObject?.Id,
            sign.GetStatus() == ObjectStatus.Created ? Guid.Empty : sign.Id,
            async () => ((await repository.Find<EvaluationSheetSign>(item =>
                item.Id == sign.Id)).FirstOrDefault()?.Act?.WorkObject?.Id, null));
        await actSignAccessValidator.EnsureCanSign(
            sign,
            nameof(EvaluationSheetSign.Act),
            workObjectAccessProvider.IsTechnicalAdministrator,
            workObjectAccessProvider.GetCurrentEmployeeId);
    }

    private async Task<Guid[]> EnsureRelatedEntity(
        Guid? loadedWorkObjectId,
        Guid entityId,
        Func<Task<(Guid? WorkObjectId, Guid? AwpId)>> getStoredRelation,
        Guid? awpId = null)
    {
        var stored = entityId == Guid.Empty ? default : await getStoredRelation();
        var storedWorkObjectId = stored.WorkObjectId;
        if (entityId != Guid.Empty && !storedWorkObjectId.HasValue)
        {
            throw new System.UnauthorizedAccessException("Связанный объект не найден.");
        }
        try
        {
            await workObjectAccessProvider.EnsureCanAccess(
                loadedWorkObjectId, entityId, () => Task.FromResult(storedWorkObjectId));
        }
        catch (System.UnauthorizedAccessException)
        {
            var relations = new[]
            {
                (WorkObjectId: loadedWorkObjectId, AwpId: awpId ?? stored.AwpId),
                (WorkObjectId: storedWorkObjectId, AwpId: stored.AwpId)
            }.Where(x => x.WorkObjectId.HasValue).Distinct().ToList();
            if (relations.Count == 0)
            {
                throw;
            }
            foreach (var relation in relations)
            {
                if (!(await permissionProvider.GetPermissions(relation.WorkObjectId, relation.AwpId)).CanReview)
                {
                    throw;
                }
            }
        }
        return new[] { loadedWorkObjectId, storedWorkObjectId }
            .Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToArray();
    }
}

public sealed class EvaluationSheetReadRestrictionProvider(
    IWorkObjectReadLimitProvider readLimitProvider,
    IEnumerable<IWorkObjectAdditionalReadLimitProvider> additionalProviders)
    : IODataReadRestrictionProvider
{
    public bool Supports(Type entityType)
        => entityType == typeof(EvaluationSheet)
            || entityType == typeof(EvaluationSheetRequirement)
            || entityType == typeof(EvaluationSheetSign);

    public async Task<ODataReadRestriction> GetRestriction(Type entityType)
    {
        var propertyPath = entityType == typeof(EvaluationSheet)
            ? nameof(EvaluationSheet.WorkObject)
            : entityType == typeof(EvaluationSheetRequirement)
                ? $"{nameof(EvaluationSheetRequirement.EvaluationSheet)}.{nameof(EvaluationSheet.WorkObject)}"
                : $"{nameof(EvaluationSheetSign.Act)}.{nameof(EvaluationSheet.WorkObject)}";
        var awpPropertyPath = entityType == typeof(EvaluationSheet) ? nameof(EvaluationSheet.Awp)
            : entityType == typeof(EvaluationSheetRequirement)
                ? $"{nameof(EvaluationSheetRequirement.EvaluationSheet)}.{nameof(EvaluationSheet.Awp)}"
                : $"{nameof(EvaluationSheetSign.Act)}.{nameof(EvaluationSheet.Awp)}";
        return await readLimitProvider.GetReadRestriction(additionalProviders, propertyPath, awpPropertyPath);
    }
}
