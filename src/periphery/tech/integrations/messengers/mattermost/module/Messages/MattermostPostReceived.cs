namespace Tmobis.Integrations.Messengers.Mattermost.Messages;

using Tmobis.Core.Medium.Models;

public sealed record MattermostPostReceived : DomainEvent
{
    public required string PostId { get; init; }

    public required string UserId { get; init; }

    public required string ChannelId { get; init; }

    public string? Text { get; init; }
}
