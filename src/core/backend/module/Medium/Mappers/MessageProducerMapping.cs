namespace Tmobis.Core.Medium.Mappers;

public record MessageProducerMapping : IMessageMapping
{
    public Type MessageType { get; set; }
    public string Name { get; set; }
}
