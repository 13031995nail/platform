namespace Tmobis.Integrations.Messengers.Telegram.Messages;

using Tmobis.Core.Medium.Models;

public sealed record SendTelegramMessage : DomainEvent
{
    public long ChatId { get; init; }

    public required string Text { get; init; }
}
