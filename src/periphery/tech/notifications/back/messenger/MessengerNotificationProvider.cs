namespace Tmobis.Notification.Messenger;

using Microsoft.Extensions.Logging;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Providers;
using Tmobis.Notification.Models;
using Tmobis.Notification.Sender;

public abstract class MessengerNotificationProvider<TMessage>(
    Guid providerId,
    string messengerCode,
    string messengerName,
    IEmployeeProvider employeeProvider,
    IEmployeeMessengerProvider employeeMessengerProvider,
    IMessageDispatcher messageDispatcher,
    ILogger logger)
    : INotificationProvider
    where TMessage : class, IMessage
{
    public async Task<IMessageResult> SendNotification(
        INotification request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = request.NotificationContents
            .Find(item => item.ProviderTypeId == providerId);
        if (content == null)
        {
            return Success();
        }

        try
        {
            var employee = await employeeProvider.GetByUser(request.User);
            if (employee == null)
            {
                logger.LogWarning(
                    "Для пользователя {UserId} не найден сотрудник",
                    request.UserId);
                return Failure();
            }

            var link = await employeeMessengerProvider.GetByEmployee(
                employee.Id,
                messengerCode);
            var preferred = await employeeMessengerProvider.GetPreferred(
                employee.Id);
            if (preferred != null
                && (link == null
                    || preferred.MessengerId != link.MessengerId))
            {
                return Success();
            }

            var message = link == null
                ? null
                : CreateMessage(link.ExternalId, content.Content);
            if (message == null)
            {
                logger.LogWarning(
                    "Для сотрудника {EmployeeId} не найден корректный идентификатор {Channel}",
                    employee.Id,
                    messengerName);
                return Failure();
            }

            await messageDispatcher.Send(message);
            logger.LogInformation(
                "Уведомление {NotificationId} отправлено через {Channel} сотруднику {EmployeeId}, идентификатор {ExternalId}",
                request.Id,
                messengerName,
                employee.Id,
                link!.ExternalId);
            return Success();
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Не удалось отправить уведомление {NotificationId} через {Channel}",
                request.Id,
                messengerName);
            return Failure();
        }
    }

    protected abstract TMessage? CreateMessage(
        string externalId,
        string content);

    private static StatusMessageResult Success()
        => new() { Error = false };

    private static StatusMessageResult Failure()
        => new() { Error = true };
}
