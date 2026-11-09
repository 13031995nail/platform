namespace Tmobis.Notification.Models;

using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Models;
using Tmobis.Notification;

public class Notification : BusinessEntity, INotification
{
    public IUser User { get; set; }

    public Guid UserId { get; set; }

    public DateTime ReadingTime { get; set; }

    public NotificationStatus Status { get; set; }

    public INotificationType Type { get; set; }

    public Guid TypeId { get; set; }

    public List<INotificationContent> NotificationContents { get; set; }
}
