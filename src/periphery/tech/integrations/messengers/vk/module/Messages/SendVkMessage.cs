namespace Tmobis.Integrations.Messengers.Vk.Messages;

using Tmobis.Core.Medium.Models;

public sealed record SendVkMessage : DomainEvent
{
    public long PeerId { get; init; }

    public required string Text { get; init; }
}
