namespace Tmobis.Integrations.Messengers.Vk.Messages;

using Newtonsoft.Json;
using Tmobis.Core.Medium.Models;

public sealed record VkCallbackReceived : DomainEvent
{
    [JsonProperty("type")]
    public string Type { get; init; } = string.Empty;

    [JsonProperty("event_id")]
    public string EventId { get; init; } = string.Empty;

    [JsonProperty("group_id")]
    public long GroupId { get; init; }

    [JsonProperty("secret")]
    public string Secret { get; init; } = string.Empty;

    [JsonProperty("object")]
    public VkCallbackObject? Object { get; init; }
}

public sealed record VkCallbackObject
{
    [JsonProperty("message")]
    public VkMessage? Message { get; init; }
}

public sealed record VkMessage
{
    [JsonProperty("from_id")]
    public long FromId { get; init; }

    [JsonProperty("peer_id")]
    public long PeerId { get; init; }

    [JsonProperty("out")]
    public int Out { get; init; }

    [JsonProperty("text")]
    public string? Text { get; init; }
}
