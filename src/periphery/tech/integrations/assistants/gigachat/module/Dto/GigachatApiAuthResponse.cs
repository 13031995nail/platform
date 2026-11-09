namespace Tmobis.Integrations.Assistants.Gigachat;

using System.Text.Json.Serialization;
using Newtonsoft.Json;

public record GigachatApiAuthResponse
{
    [JsonPropertyName("access_token")]
    [JsonProperty(PropertyName = "access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    [JsonProperty(PropertyName = "expires_at")]
    public long ExpiresAt { get; init; }
}