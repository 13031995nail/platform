namespace Tmobis.Core.Medium.Producers;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

public class MessageDispatcher(
    IServiceProvider services,
    IMessageProducerRegister producerRegister,
    [FromKeyedServices("Default")] IMessageProducer? defaultProducer = null)
: IMessageDispatcher
{
    public async Task<IMessageResult> SendWithResult(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var producer = GetProducer(message);
        return await producer.SendWithResult(message);
    }

    public async Task<TResult> SendWithResult<TRequest, TResult>(
        TRequest message)
        where TRequest : IMessage<TResult>
        where TResult : class, IMessageResult
    {
        return await SendWithResult(message) as TResult
            ?? throw new InvalidOperationException(
                "Не удалось получить результат");
    }

    public async Task Send(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var producer = GetProducer(message);
        await producer.Send(message);
    }

    public async Task Send<TRequest>(TRequest message)
        where TRequest : IMessage
    {
        await Send(message as IMessage);
    }

    public async Task Publish(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var producer = GetProducer(message);
        await producer.Publish(message);
    }

    public async Task Publish<TRequest>(TRequest message)
        where TRequest : IMessage
    {
        await Publish(message as IMessage);
    }

    public async Task<List<TResult>> PublishChainWithResult<TRequest, TResult>(
        TRequest message)
        where TRequest : IMessage<TResult>
        where TResult : class, IChainMessageResult
    {
        return [.. (await PublishChainWithResult(message)).Cast<TResult>()];
    }

    public async Task<List<IChainMessageResult>> PublishChainWithResult(
        IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var producer = GetProducer(message) as IChainPublishProducer
            ?? throw new InvalidOperationException(
                "Не найден Producer");
        return await producer.PublishChainWithResult(message)
            ?? throw new InvalidOperationException(
                @$"{nameof(producer)} не поддерживает
                 {nameof(PublishChainWithResult)}");
    }

    private IMessageProducer GetProducer(object message)
    {
        var producer = GetProducerFromMappings(message.GetType());
        if (producer == null)
        {
            if (defaultProducer == null)
            {
                throw new InvalidOperationException(
                "No suitable IMessageProducer found.");
            }

            return defaultProducer;
        }

        return producer;
    }

    private IMessageProducer? GetProducerFromMappings(Type messageType)
    {
        Type? producerType = producerRegister.GetProducerByMessageType(messageType);
        return producerType != null
            ? services.GetService(producerType) as IMessageProducer
            : null;
    }
}
