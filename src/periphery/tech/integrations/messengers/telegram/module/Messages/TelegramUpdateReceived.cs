namespace Tmobis.Integrations.Messengers.Telegram.Messages;

using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Tmobis.Core.Medium.Models;
using Tmobis.Integrations.Messengers.Telegram.Authorization;

[Authorize(Policy = TelegramAuthorizationPolicies.Webhook)]
[MessageMetadata("Telegram", "webhook")]
public record TelegramUpdateReceived : DomainEvent
{
    [JsonProperty("update_id")]
    public long UpdateId { get; init; }

    [JsonProperty("message")]
    public TelegramMessage? Message { get; init; }
}

public record TelegramMessage
{
    [JsonProperty("from")]
    public TelegramUser? From { get; init; }

    [JsonProperty("chat")]
    public TelegramChat? Chat { get; init; }

    [JsonProperty("text")]
    public string? Text { get; init; }

    [JsonProperty("contact")]
    public TelegramContact? Contact { get; init; }
}

public record TelegramUser
{
    [JsonProperty("id")]
    public long Id { get; init; }
}

public record TelegramChat
{
    [JsonProperty("id")]
    public long Id { get; init; }
}

public record TelegramContact
{
    [JsonProperty("phone_number")]
    public string PhoneNumber { get; init; } = string.Empty;

    [JsonProperty("user_id")]
    public long? UserId { get; init; }
}
