namespace Tmobis.Notification.WebSocket;

using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.Signalr;
using Tmobis.Notification.Models;
using Tmobis.Notification.Sender;

public class NotificationWebSocketProvider(
    IOptions<NotificationWebSocketOptions> options,
    IMessageDispatcher messageDispatcher
) : INotificationProvider
{
    private readonly NotificationWebSocketOptions options = options.Value;
    public async Task<IMessageResult> SendNotification(INotification request)
    {
        try
        {
            var content = request.NotificationContents
            .Find(x => x.ProviderTypeId == options.ProviderId);
            if (content != null)
            {
                SendNotification notification = new SendNotification
                {
                    Title = content.Title,
                    Content = content.Content,
                };
                notification.Context[SignalRMessageContext.Target] = request.UserId;
                await messageDispatcher.Send(notification);
            }
            return new StatusMessageResult { Error = false };

        }
        catch (Exception ex) {
            return new StatusMessageResult { Error = true };
        }
    }
    
}
