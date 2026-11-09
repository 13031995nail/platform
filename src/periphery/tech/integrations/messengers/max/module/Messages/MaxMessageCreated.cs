namespace Tmobis.Integrations.Messengers.Max.Messages;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Tmobis.Core.Medium.Models;
using Tmobis.Integrations.Messengers.Max.Authorization;

[Authorize(Policy = MaxAuthorizationPolicies.Webhook)]
[MessageMetadata("webhooks", "max")]
public record MaxMessageCreated : DomainEvent
{
    [JsonProperty("update_type")]
    public string UpdateType { get; init; } = string.Empty;

    [JsonProperty("timestamp")]
    public long Timestamp { get; init; }

    [JsonProperty("message")]
    public MaxMessage? Message { get; init; }
}

public record MaxMessage
{
    [JsonProperty("sender")]
    public MaxUser? Sender { get; init; }

    [JsonProperty("recipient")]
    public MaxRecipient? Recipient { get; init; }

    [JsonProperty("body")]
    public MaxMessageBody? Body { get; init; }
}

public record MaxUser
{
    [JsonProperty("user_id")]
    public long UserId { get; init; }
}

public record MaxRecipient
{
    [JsonProperty("chat_id")]
    public long ChatId { get; init; }
}

public record MaxMessageBody
{
    [JsonProperty("text")]
    public string? Text { get; init; }

    [JsonProperty("attachments")]
    public List<MaxAttachment>? Attachments { get; init; }
}

public record MaxAttachment
{
    [JsonProperty("type")]
    public string Type { get; init; } = string.Empty;

    [JsonProperty("payload")]
    public MaxContactPayload? Payload { get; init; }
}

public record MaxContactPayload
{
    [JsonProperty("vcf_info")]
    public string VcfInfo { get; init; } = string.Empty;

    [JsonProperty("hash")]
    public string Hash { get; init; } = string.Empty;
}
