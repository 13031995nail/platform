namespace Tmobis.Integrations.Messengers.Messages;

using Tmobis.Core.Medium.Models;

public sealed record MessengerMessageReceived
    : DomainCommand<MessengerMessageResult>
{
    public string MessengerCode { get; init; } = string.Empty;

    public string MessengerName { get; init; } = string.Empty;

    public string ExternalId { get; init; } = string.Empty;

    public bool HasContact { get; init; }

    public bool ContactIsTrusted { get; init; }

    public bool AllowUntrustedContact { get; init; }

    public string? Phone { get; init; }
}

public sealed record MessengerMessageResult : IMessageResult
{
    public MessengerResponseAction Action { get; init; }

    public string? Text { get; init; }
}

public enum MessengerResponseAction
{
    None = 0,
    SendMessage = 1,
    RequestContact = 2
}
