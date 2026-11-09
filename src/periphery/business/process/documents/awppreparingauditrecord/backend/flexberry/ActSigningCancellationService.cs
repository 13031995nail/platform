namespace Tmobis.Business.Process.Documents;

using System.Linq.Expressions;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Process.Documents.Base.Flexberry;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

/// <summary>
/// Атомарная отмена текущего цикла подписания назначенным базовым подписантом.
/// </summary>
public sealed class ActSigningCancellationService(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    RedinessActAccess actAccess,
    AwpReadinessPassportAccess passportAccess,
    AwpAssessingProgramAccess programAccess,
    EvaluationSheetAccess sheetAccess)
{
    public Task Cancel(string signModel, Guid documentId)
        => signModel switch
        {
            "readiness-act-sign" => Cancel<RedinessAct, ReadinessActSign>(documentId,
                sign => sign.Act != null && sign.Act.Id == documentId,
                ISignRole.Keys.CommissionChair, actAccess.EnsureCanChange, actAccess.EnsureCanChange),
            "awp-readiness-passport-sign" => Cancel<AwpReadinessPassport, AwpReadinessPassportSign>(documentId,
                sign => sign.Act != null && sign.Act.Id == documentId,
                ISignRole.Keys.CommissionCreator, passportAccess.EnsureCanChange, passportAccess.EnsureCanChange),
            "awp-assessing-program-sign" => Cancel<AwpAssessingProgram, AwpAssessingProgramSign>(documentId,
                sign => sign.Act != null && sign.Act.Id == documentId,
                ISignRole.Keys.CommissionChair, programAccess.EnsureCanChange, programAccess.EnsureCanChange),
            "evaluation-sheet-sign" => Cancel<EvaluationSheet, EvaluationSheetSign>(documentId,
                sign => sign.Act != null && sign.Act.Id == documentId,
                ISignRole.Keys.CommissionMember, sheetAccess.EnsureCanChange, sheetAccess.EnsureCanChange),
            _ => throw new ArgumentException("Неизвестный тип документа."),
        };

    private async Task Cancel<TDocument, TSign>(Guid documentId,
        Expression<Func<TSign, bool>> predicate, Guid baseRoleId,
        Func<TDocument, Task> ensureDocumentAccess, Func<TSign, Task> ensureSignAccess)
        where TDocument : BaseActBase
        where TSign : ActSign
    {
        var atomicRepository = repository as IAtomicRepository
            ?? throw new NotSupportedException("Репозиторий не поддерживает атомарную отмену подписания.");
        var document = await repository.GetById<TDocument>(documentId, new EntityView("AuditView"))
            ?? throw new UnauthorizedAccessException("Документ недоступен.");
        var signs = await repository.Find(predicate, view: new EntityView("AuditView"));
        var employeeId = await accessProvider.GetCurrentEmployeeId();
        if (!await accessProvider.IsTechnicalAdministrator()
            && (!employeeId.HasValue || !signs.Any(sign => sign.Employee?.Id == employeeId
                && sign.SignRole?.Id == baseRoleId)))
        {
            throw new UnauthorizedAccessException("Отменить подписание может только назначенный базовый подписант.");
        }
        if (document.ActStatus?.Id != IActStatus.Keys.Signing)
        {
            throw new ArgumentException("Отмена разрешена только для документа на подписании.");
        }
        var editing = await repository.GetById<ActStatus>(IActStatus.Keys.Editing)
            ?? throw new InvalidOperationException("Не найден статус редактирования.");
        var oldStatus = document.ActStatus;
        var oldValues = signs.Select(sign => (Sign: sign, sign.SignDate, sign.Signature)).ToList();
        var changedSigns = oldValues.Where(item => item.SignDate.HasValue
            || !string.IsNullOrEmpty(item.Signature)).Select(item => item.Sign).ToList();
        using var cancellation = ActSigningCancellationScope.Enter(document, signs);
        try
        {
            document.ActStatus = editing;
            foreach (var sign in changedSigns)
            {
                sign.SignDate = null;
                sign.Signature = null;
            }
            await ensureDocumentAccess(document);
            foreach (var sign in changedSigns)
            {
                await ensureSignAccess(sign);
            }
            repository.Update(changedSigns.Cast<IEntity>().Append(document).ToList());
            // Выполнение одного UpdateObjects: подписи и статус сохраняются в транзакции Flexberry ORM.
            await atomicRepository.CommitAtomically();
        }
        catch
        {
            document.ActStatus = oldStatus;
            foreach (var item in oldValues)
            {
                item.Sign.SignDate = item.SignDate;
                item.Sign.Signature = item.Signature;
            }
            throw;
        }
    }
}
