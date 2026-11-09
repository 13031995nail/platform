namespace Tmobis.Core.Medium.Mappers;

public interface IMessageMapping
{
    public Type MessageType { get; set; }
    public string Name { get; set; }
}

