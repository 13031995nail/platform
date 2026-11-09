namespace Tmobis.Notification.Sender;

using Tmobis.Core.Medium.Models;
using Tmobis.Notification.Models;

public interface INotificationProvider
{
    Task<IMessageResult> SendNotification(INotification request);
}
