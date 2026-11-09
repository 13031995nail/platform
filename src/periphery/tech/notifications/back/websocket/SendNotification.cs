namespace Tmobis.Notification.WebSocket;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "notification")]
public record SendNotification : RemoteEvent
{
    public required string Title { get; init; }

    public required string Content { get; init; }
}
