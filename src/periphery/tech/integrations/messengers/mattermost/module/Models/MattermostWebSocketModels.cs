namespace Tmobis.Integrations.Messengers.Mattermost.Models;

using System.Text.Json.Serialization;

internal sealed record MattermostWebSocketMessage
{
    [JsonPropertyName("event")]
    public string? Event { get; init; }

    [JsonPropertyName("data")]
    public MattermostWebSocketData? Data { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("seq_reply")]
    public long? SequenceReply { get; init; }
}

internal sealed record MattermostWebSocketData
{
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; init; }

    [JsonPropertyName("post")]
    public string? Post { get; init; }
}

internal sealed record MattermostPost
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = string.Empty;

    [JsonPropertyName("channel_id")]
    public string ChannelId { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
