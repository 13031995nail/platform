namespace Tmobis.Notification;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "notification")]
public record CreateNotification : RemoteEvent
{
    public required Guid UserId { get; init; }

    public required Guid TypeId { get; init; }

    public List<CreateNotificationContent> Content { get; init; } = [];
}

public record CreateNotificationContent
{
    public required string Title { get; init; }

    public required string Content { get; init; }

    public required Guid ProviderTypeId { get; init; }
}
