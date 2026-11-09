namespace Tmobis.Business.Process.Documents.Flexberry;

using System.Linq.Expressions;
using ICSSoft.STORMNET;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

/// <summary>
/// Проверяет право текущего пользователя на подписание документа.
/// </summary>
public sealed class ActSignAccessValidator(IRepository repository)
{
    private static readonly string[] SigningProperties =
    [
        nameof(ActSign.Signature),
        nameof(ActSign.SignDate),
        nameof(ActSign.SignType),
    ];

    public bool IsSigningChange(ActSign sign)
        => sign.GetStatus() == ObjectStatus.Altered
            && sign.GetAlteredPropertyNames().Intersect(SigningProperties).Any();

    public async Task EnsureCurrentEmployeeAssigned<TSign>(
        Func<Guid, Expression<Func<TSign, bool>>> assignmentPredicate,
        Func<Task<Guid?>> getCurrentEmployeeId)
        where TSign : ActSign
    {
        var employeeId = await getCurrentEmployeeId();
        var isAssignedSigner = employeeId.HasValue
            && await repository.Any(assignmentPredicate(employeeId.Value));
        if (!isAssignedSigner)
        {
            throw new UnauthorizedAccessException(
                "Подписать документ может только назначенный подписант.");
        }
    }

    public async Task EnsureCanSign<TSign>(
        TSign sign,
        string documentPropertyName,
        Func<Task<bool>> isTechnicalAdministrator,
        Func<Task<Guid?>> getCurrentEmployeeId)
        where TSign : ActSign
    {
        EnsureSignatureStateIsConsistent(sign);

        if (await isTechnicalAdministrator())
        {
            return;
        }

        if (sign.GetStatus() == ObjectStatus.Created)
        {
            if (!string.IsNullOrWhiteSpace(sign.Signature) || sign.SignDate.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Подписание документа при создании записи подписанта запрещено.");
            }

            return;
        }

        var immutableAssignmentProperties = new[]
        {
            nameof(sign.Employee),
            nameof(sign.SignRole),
            documentPropertyName,
        };
        if (sign.GetAlteredPropertyNames().Intersect(immutableAssignmentProperties).Any())
        {
            throw new UnauthorizedAccessException(
                "Назначенного подписанта, его роль и документ нельзя изменять.");
        }

        if (!IsSigningChange(sign))
        {
            return;
        }

        if (ActSigningCancellationScope.AllowsSign(sign))
        {
            return;
        }

        await EnsureCurrentEmployeeAssigned<TSign>(employeeId => item =>
                item.Id == sign.Id
                && item.Employee != null
                && item.Employee.Id == employeeId,
            getCurrentEmployeeId);
    }

    private static void EnsureSignatureStateIsConsistent(ActSign sign)
    {
        if (sign.SignDate.HasValue != !string.IsNullOrWhiteSpace(sign.Signature))
        {
            throw new UnauthorizedAccessException(
                "Дата подписания и подпись должны быть заполнены одновременно.");
        }
    }

    public async Task<(bool AllSigned, bool CurrentEmployeeIsBaseSigner)> GetSigningStatus<TSign>(
        Expression<Func<TSign, bool>> documentPredicate,
        Func<Task<Guid?>> getCurrentEmployeeId,
        Guid baseSignRoleId)
        where TSign : ActSign
    {
        var employeeId = await getCurrentEmployeeId();
        var signs = await repository.Find(documentPredicate,
            view: new EntityView("AuditView"));
        if (!employeeId.HasValue || !signs.Any(sign => sign.Employee?.Id == employeeId))
        {
            throw new UnauthorizedAccessException(
                "Подписать документ может только назначенный подписант.");
        }

        return (signs.Count > 0 && signs.All(sign => sign.SignDate.HasValue
                && !string.IsNullOrWhiteSpace(sign.Signature)),
            signs.Any(sign => sign.Employee?.Id == employeeId
                && sign.SignRole?.Id == baseSignRoleId));
    }
}
