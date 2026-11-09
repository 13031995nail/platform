namespace Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Models;

public interface IMessageDispatcher
: IMessageProducer, IChainPublishProducer
{
    Task<TResult> SendWithResult<TRequest, TResult>(TRequest message)
        where TRequest : IMessage<TResult>
        where TResult : class, IMessageResult;

    Task Send<TRequest>(TRequest message)
        where TRequest : IMessage;

    Task Publish<TRequest>(TRequest message)
        where TRequest : IMessage;

    Task<List<TResult>> PublishChainWithResult<TRequest, TResult>(TRequest message)
        where TRequest : IMessage<TResult>
        where TResult : class, IChainMessageResult;
}
