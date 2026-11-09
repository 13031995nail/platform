namespace Tmobis.Notification.Models;

using Tmobis.Core.Models;
using Tmobis.Notification.Models;

public class NotificationContent : BusinessEntity, INotificationContent
{
    public Guid ProviderTypeId { get; set; }

    public INotificationProviderType ProviderType { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public Guid NotificationId { get; set; }

    public INotification Notification { get; set; }
}
