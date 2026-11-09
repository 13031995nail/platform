namespace Tmobis.Notification.Telegram;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Providers;
using Tmobis.Integrations.Messengers.Telegram;
using Tmobis.Integrations.Messengers.Telegram.Messages;
using Tmobis.Notification.Messenger;

public sealed class NotificationTelegramProvider(
    IOptions<NotificationTelegramOptions> optionsAccessor,
    IEmployeeProvider employeeProvider,
    IEmployeeMessengerProvider employeeMessengerProvider,
    IMessageDispatcher messageDispatcher,
    ILogger<NotificationTelegramProvider> logger)
    : MessengerNotificationProvider<SendTelegramMessage>(
        optionsAccessor.Value.ProviderId,
        TelegramOptions.MessengerCode,
        TelegramOptions.MessengerName,
        employeeProvider,
        employeeMessengerProvider,
        messageDispatcher,
        logger)
{
    protected override SendTelegramMessage? CreateMessage(
        string externalId,
        string content)
        => long.TryParse(
            externalId,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var chatId)
            ? new SendTelegramMessage { ChatId = chatId, Text = content }
            : null;
}
