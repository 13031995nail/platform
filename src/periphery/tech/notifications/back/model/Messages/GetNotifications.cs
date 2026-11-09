namespace Tmobis.Notification.Messages;

using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;
using Tmobis.Notification.Models;

[MessageMetadata("Notification", "GetNotifications")]
public sealed record GetNotifications
    : GetPagedEntityListQuery<INotification>
{
    public const int DefaultPerPage = 20;

    public const int MaxPerPage = 100;

    public GetNotifications()
    {
        PerPage = DefaultPerPage;
    }
}
