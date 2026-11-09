namespace Tmobis.Core.Medium.Metas;

using Tmobis.Core.Medium.Models;

public interface IMetaMessage : IMessage
{
    string? RawMessage { get; }
}

public record MetaMessage : Message, IMetaMessage
{
    public string? RawMessage { get; init; }
}
