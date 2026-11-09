namespace Tmobis.Notification.Models;

using Tmobis.Core.Models;

public interface INotificationContent : IBusinessEntity, IRegisteredEntity
{
  public Guid ProviderTypeId { get; set; }

  public INotificationProviderType ProviderType { get; set; }

  public string Title { get; set; }

  public string Content { get; set; }

  public Guid NotificationId { get; set; }

  public INotification Notification { get; set; }
}
