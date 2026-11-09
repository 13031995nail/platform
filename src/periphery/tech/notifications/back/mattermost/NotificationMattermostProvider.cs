namespace Tmobis.Notification.Mattermost;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Mattermost;
using Tmobis.Integrations.Messengers.Mattermost.Messages;
using Tmobis.Integrations.Messengers.Providers;
using Tmobis.Notification.Messenger;

public sealed class NotificationMattermostProvider(
    IOptions<NotificationMattermostOptions> optionsAccessor,
    IEmployeeProvider employeeProvider,
    IEmployeeMessengerProvider employeeMessengerProvider,
    IMessageDispatcher messageDispatcher,
    ILogger<NotificationMattermostProvider> logger)
    : MessengerNotificationProvider<SendMattermostMessage>(
        optionsAccessor.Value.ProviderId,
        MattermostOptions.MessengerCode,
        MattermostOptions.MessengerName,
        employeeProvider,
        employeeMessengerProvider,
        messageDispatcher,
        logger)
{
    protected override SendMattermostMessage? CreateMessage(
        string externalId,
        string content)
        => string.IsNullOrWhiteSpace(externalId)
            ? null
            : new SendMattermostMessage
            {
                UserId = externalId,
                Text = content
            };
}
