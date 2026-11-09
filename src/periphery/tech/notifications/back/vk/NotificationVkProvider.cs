namespace Tmobis.Notification.Vk;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Providers;
using Tmobis.Integrations.Messengers.Vk;
using Tmobis.Integrations.Messengers.Vk.Messages;
using Tmobis.Notification.Messenger;

public sealed class NotificationVkProvider(
    IOptions<NotificationVkOptions> optionsAccessor,
    IEmployeeProvider employeeProvider,
    IEmployeeMessengerProvider employeeMessengerProvider,
    IMessageDispatcher messageDispatcher,
    ILogger<NotificationVkProvider> logger)
    : MessengerNotificationProvider<SendVkMessage>(
        optionsAccessor.Value.ProviderId,
        VkOptions.MessengerCode,
        VkOptions.MessengerName,
        employeeProvider,
        employeeMessengerProvider,
        messageDispatcher,
        logger)
{
    protected override SendVkMessage? CreateMessage(
        string externalId,
        string content)
        => long.TryParse(
            externalId,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var peerId)
            ? new SendVkMessage { PeerId = peerId, Text = content }
            : null;
}
