namespace Tmobis.Medium.RabbitMQ.SecondaryAdapters;

using Microsoft.Extensions.Logging;
using Tmobis.Medium.RabbitMQ.Client;
using global::RabbitMQ.Client;
using Tmobis.Core.Medium.Models;
using Microsoft.Extensions.Options;

public class RabbitProducer(
    IRabbitAdapter rabbitAdapter,
    ILogger<MetaRabbitProducer> logger,
    IOptions<RabbitProducerOptions> options)
    : RabbitProducerBase(rabbitAdapter, logger, options)
{
    private readonly IRabbitAdapter rabbitAdapter = rabbitAdapter;
    private readonly RabbitProducerOptions options = options.Value;

    protected async override Task InitModel(IMessage message)
    {
        await base.InitModel(message);

        string? queueName = message?.GetType().FullName?.ToLower();
        if (string.IsNullOrEmpty(queueName))
        {
            logger.LogWarning("У типа сообщения {Type} не существует полного имени. " +
                "Создание очереди пропущено", message?.GetType());
            return;
        }

        if (string.IsNullOrEmpty(options.ExchangeName))
        {
            return;
        }
        if (options.DlxExchangeName != null)
        {
            await rabbitAdapter.CreateQueueWithDLX(rabbitAdapter.Channel, queueName, options.DlxExchangeName);
        }
        else
        {
            await rabbitAdapter.CreateQueue(rabbitAdapter.Channel, queueName);
        }
        await rabbitAdapter.BindQueue(rabbitAdapter.Channel, queueName, options.ExchangeName);
    }

    protected async override Task SendToChannel(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (rabbitAdapter.Channel == null || rabbitAdapter.Channel.IsClosed)
        {
            return;
        }

        var messageType = message.GetType();
        var routingKey = message.GetType().FullName?.ToLower();

        BasicProperties props = new()
        {
            Type = messageType.FullName,
            ContentType = "application/json"
        };

        if (string.IsNullOrEmpty(routingKey) || string.IsNullOrEmpty(options.ExchangeName))
        {
            return;
        }

        byte[] preparedMessage = PrepareMessage(message);

        await rabbitAdapter.Channel.BasicPublishAsync(
            exchange: options.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: preparedMessage
        );
    }
}

