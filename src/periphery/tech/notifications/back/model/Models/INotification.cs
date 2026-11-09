namespace Tmobis.Notification.Models;

using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

[InternalEntity(EntityVisibility.Read)]
public interface INotification : IBusinessEntity, IRegisteredEntity
{
    public IUser User { get; set; }

    public Guid UserId { get; set; }

    public DateTime ReadingTime { get; set; }

    public NotificationStatus Status { get; set; }

    public INotificationType Type { get; set; }

    public Guid TypeId { get; set; }

    public List<INotificationContent> NotificationContents { get; set; }
}
