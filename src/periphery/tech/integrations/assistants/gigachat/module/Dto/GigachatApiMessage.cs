namespace Tmobis.Integrations.Assistants.Gigachat;

public record GigachatApiMessage
{
    public string Role { get; init; } = "user";

    public required string Content { get; init; }
    
    public List<string> Attachments { get; init; } = [];
}