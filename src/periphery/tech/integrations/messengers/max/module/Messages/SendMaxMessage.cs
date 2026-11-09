namespace Tmobis.Integrations.Messengers.Max.Messages;

using Tmobis.Core.Medium.Models;

public sealed record SendMaxMessage : DomainEvent
{
    public long ChatId { get; init; }

    public required string Text { get; init; }
}
