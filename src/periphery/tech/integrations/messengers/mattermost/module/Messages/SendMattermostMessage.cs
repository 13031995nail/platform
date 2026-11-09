namespace Tmobis.Integrations.Messengers.Mattermost.Messages;

using Tmobis.Core.Medium.Models;

public sealed record SendMattermostMessage : DomainEvent
{
    public required string UserId { get; init; }

    public required string Text { get; init; }
}
