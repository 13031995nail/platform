namespace Tmobis.Notification.Messages;

using Tmobis.Core.Medium.Models;

[MessageMetadata("Notification", "GetUnreadNotificationCount")]
public record GetUnreadNotificationCount
    : DomainQuery<GetUnreadNotificationCountResult>;

public record GetUnreadNotificationCountResult : IMessageResult
{
    public int Count { get; init; }
}
