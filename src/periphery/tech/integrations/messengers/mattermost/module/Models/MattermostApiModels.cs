namespace Tmobis.Integrations.Messengers.Mattermost.Models;

using System.Text.Json.Serialization;

internal sealed record MattermostChannel
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
}

internal sealed record MattermostCreatePost
{
    [JsonPropertyName("channel_id")]
    public string ChannelId { get; init; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
