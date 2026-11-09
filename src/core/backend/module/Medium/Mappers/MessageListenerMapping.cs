namespace Tmobis.Core.Medium.Mappers;

public record MessageListenerMapping : IMessageMapping
{
    public required Type MessageType { get; set; }
    public required string Name { get; set; }
}
