namespace Tmobis.Integrations.Assistants.Gigachat;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public record GigachatApiRequest
{
    [JsonPropertyName("model")]
    [JsonProperty(PropertyName = "model")]
    public required string Model { get; init; }

    [JsonPropertyName("profanity_check")]
    [JsonProperty(PropertyName = "profanity_check")]
    public bool ProfanityCheck { get; init; } = true;

    public List<GigachatApiMessage> Messages { get; init; } = [];
}