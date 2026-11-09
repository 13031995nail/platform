namespace Tmobis.Core.Medium.Metas;

using Tmobis.Core.Medium.Models;

public interface IWrappedMessage : IMessage
{
    public IMessage? Wrappee { get; }
}

public record WrappedMessage : Message, IWrappedMessage
{
    public IMessage? Wrappee { get; init; }
}

public record WrappedMessage<T>
: Message, IWrappedMessage
where T : IMessage
{
    public T? Wrappee { get; init; }

    IMessage? IWrappedMessage.Wrappee => Wrappee;
}
