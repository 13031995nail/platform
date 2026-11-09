namespace Tmobis.Notification.Max;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Max;
using Tmobis.Integrations.Messengers.Max.Messages;
using Tmobis.Integrations.Messengers.Providers;
using Tmobis.Notification.Messenger;

public sealed class NotificationMaxProvider(
    IOptions<NotificationMaxOptions> optionsAccessor,
    IEmployeeProvider employeeProvider,
    IEmployeeMessengerProvider employeeMessengerProvider,
    IMessageDispatcher messageDispatcher,
    ILogger<NotificationMaxProvider> logger)
    : MessengerNotificationProvider<SendMaxMessage>(
        optionsAccessor.Value.ProviderId,
        MaxOptions.MessengerCode,
        MaxOptions.MessengerName,
        employeeProvider,
        employeeMessengerProvider,
        messageDispatcher,
        logger)
{
    protected override SendMaxMessage? CreateMessage(
        string externalId,
        string content)
        => long.TryParse(
            externalId,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var chatId)
            ? new SendMaxMessage { ChatId = chatId, Text = content }
            : null;
}
