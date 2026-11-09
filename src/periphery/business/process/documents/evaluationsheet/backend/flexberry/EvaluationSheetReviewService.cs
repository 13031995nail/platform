namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Address;
using Tmobis.Core.Repositories;

/// <summary>
/// Передача и приёмка существующего оценочного листа независимо от ролевого маршрута.
/// </summary>
public sealed class EvaluationSheetReviewService(
    IRepository repository,
    IUnitOfWork unitOfWork,
    EvaluationSheetPermissionProvider permissionProvider)
{
    public Task<EvaluationSheetPermissions> GetPermissions(
        Guid? workObjectId = null, Guid? awpId = null, WorkObject? workObject = null)
        => permissionProvider.GetPermissions(workObjectId, awpId, workObject);

    public async Task Review(Guid sheetId, string operation)
    {
        ValidateOperation(operation);
        var sheet = await repository.GetById<EvaluationSheet>(sheetId,
            EvaluationSheet.Views.EvaluationSheetEEntityView)
            ?? throw new ArgumentException("Оценочный лист не найден.");
        var permissions = await GetPermissions(sheet.WorkObject.Id, sheet.Awp.Id);
        if (operation == "submit" ? !permissions.CanSubmit : !permissions.CanReview)
        {
            throw new UnauthorizedAccessException();
        }
        if (!CanChangeReview(sheet))
        {
            throw new ArgumentException("Подписанный или утверждённый документ нельзя изменить.");
        }
        switch (operation)
        {
            case "submit":
                if (!CanSubmit(sheet))
                {
                    throw new ArgumentException("Результат уже передан на проверку или принят.");
                }
                await EnsureLinkedTso(sheet.WorkObject.Id);
                sheet.ReviewRequestedAt = DateTime.UtcNow;
                sheet.ReviewStartedAt = null;
                break;
            case "review":
                if (sheet.ReviewRequestedAt == null)
                {
                    throw new ArgumentException("Перед началом проверки результат должен быть отправлен ТСО.");
                }
                if (!CanStartReview(sheet))
                {
                    throw new ArgumentException("Проверка уже начата или результат принят.");
                }
                sheet.ReviewStartedAt = DateTime.UtcNow;
                break;
            case "accept":
                if (!CanAccept(sheet))
                {
                    throw new ArgumentException("Перед приёмкой необходимо начать проверку.");
                }
                Accept(sheet);
                break;
        }
        repository.Update(sheet);
        await unitOfWork.Commit();
    }

    public static void ValidateOperation(string operation)
    {
        if (operation is not ("submit" or "review" or "accept"))
        {
            throw new ArgumentException("Неизвестное действие проверки.");
        }
    }

    public async Task EnsureCanRequestReview(Guid workObjectId, Guid? awpId = null, bool allowReviewer = true)
    {
        var permissions = await GetPermissions(workObjectId, awpId);
        if (allowReviewer && permissions.CanReview)
        {
            return;
        }
        if (!permissions.CanSubmit)
        {
            throw new UnauthorizedAccessException();
        }
        await EnsureLinkedTso(workObjectId);
    }

    private async Task EnsureLinkedTso(Guid workObjectId)
    {
        var links = await repository.Find<WorkObjectTsoOrganization>(
            x => x.WorkObject.Id == workObjectId);
        if (links.Count == 0)
        {
            throw new ArgumentException("Для передачи результата необходимо связать объект с ТСО.");
        }
    }

    public static bool CanChangeReview(EvaluationSheet? sheet)
        => sheet != null && sheet.ActStatus?.Id != IActStatus.Keys.Signed
            && sheet.ActStatus?.Id != IActStatus.Keys.Approved;

    public static bool IsAccepted(EvaluationSheet? sheet,
        IEnumerable<EvaluationSheetRequirement>? requirements = null)
    {
        var items = requirements ?? sheet?.Requirements.Cast<EvaluationSheetRequirement>() ?? [];
        return sheet?.IsVerifiedTSO == true && EvaluationSheetProgress.Calculate(items).Total > 0
            && !items.Any(x => !string.IsNullOrWhiteSpace(x.ProblemComment) && !x.IsCompletedReq)
            && items.Where(EvaluationSheetProgress.IsCounted).All(x => x.IsVerified);
    }

    public static bool CanSubmit(EvaluationSheet? sheet,
        IEnumerable<EvaluationSheetRequirement>? requirements = null)
    {
        var items = requirements ?? sheet?.Requirements.Cast<EvaluationSheetRequirement>() ?? [];
        return CanChangeReview(sheet) && !IsAccepted(sheet, items)
            && (sheet!.ReviewRequestedAt == null && sheet.ReviewStartedAt == null || items.Any(x =>
                !string.IsNullOrWhiteSpace(x.ProblemComment) && !x.IsCompletedReq));
    }

    public static bool CanStartReview(EvaluationSheet? sheet,
        IEnumerable<EvaluationSheetRequirement>? requirements = null)
        => CanChangeReview(sheet) && sheet!.ReviewRequestedAt != null
            && sheet.ReviewStartedAt == null && !IsAccepted(sheet, requirements);

    public static bool CanAccept(EvaluationSheet? sheet,
        IEnumerable<EvaluationSheetRequirement>? requirements = null)
        => CanChangeReview(sheet) && sheet!.ReviewStartedAt != null && !IsAccepted(sheet, requirements);

    private static void Accept(EvaluationSheet sheet)
    {
        var requirements = sheet.Requirements.Cast<EvaluationSheetRequirement>().ToList();
        var progress = EvaluationSheetProgress.Calculate(requirements);
        if (progress.Total == 0 || progress.Filled != progress.Total
            || requirements.Any(x => !string.IsNullOrWhiteSpace(x.ProblemComment) && !x.IsCompletedReq))
        {
            throw new ArgumentException("Заполните требования и устраните замечания перед приёмкой.");
        }
        foreach (var requirement in requirements.Where(EvaluationSheetProgress.IsCounted))
        {
            requirement.IsVerified = true;
        }
        sheet.IsVerifiedTSO = true;
    }
}
