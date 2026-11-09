namespace Tmobis.Core.Medium.Consumers;

using Tmobis.Core.Medium.Models;

public interface IMessageConsumer<in TRequest, TResult>
    where TRequest : IMessage<TResult>
    where TResult : IMessageResult
{
    public Task<TResult> ConsumeWithResult(TRequest message);
}

public interface IMessageConsumer<in TRequest>
    where TRequest : IMessage
{
    public Task Consume(TRequest message);
}
