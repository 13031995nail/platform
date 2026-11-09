namespace Tmobis.Medium.WebApi.Producer;

public class WebApiProducerOptions
{
    public const string WebApiProducer = "WebApiProducer";

    public Dictionary<string, RemoteApiSettings> Types { get; set; } = [];
}

public record RemoteApiSettings
{
    required public string Address { get; init; }

    public bool IsInternal { get; init; } = true;

    public string? Group { get; init; }

    public string? Name { get; init; }
}
