namespace Tmobis.Integrations.Assistants.Gigachat;

public record GigachatApiResponse
{    
    public List<GigachatApiResponseChoice> Choices { get; init; } = [];
}

public record GigachatApiResponseChoice
{
    public GigachatApiMessage? Message { get; init; }
}