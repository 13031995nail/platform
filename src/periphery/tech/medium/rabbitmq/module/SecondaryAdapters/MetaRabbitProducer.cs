namespace Tmobis.Medium.RabbitMQ.SecondaryAdapters;

using Tmobis.Core.Medium.Metas;
using global::RabbitMQ.Client;
using Tmobis.Medium.RabbitMQ.Models;
using Microsoft.Extensions.Logging;
using Tmobis.Medium.RabbitMQ.Client;
using Tmobis.Core.Medium.Models;
using Microsoft.Extensions.Options;
using System.Text;

public class MetaRabbitProducer(
    IRabbitAdapter rabbitAdapter,
    ILogger<MetaRabbitProducer> logger,
    IOptions<RabbitProducerOptions> options)
    : RabbitProducerBase(rabbitAdapter, logger, options)
{
    private readonly IRabbitAdapter rabbitAdapter = rabbitAdapter;
    private readonly RabbitProducerOptions options = options.Value;

    protected override byte[] PrepareMessage(IMessage message)
    {
        if (message is RabbitMetaMessage rabbitMeta)
        {
          return Encoding.UTF8.GetBytes(rabbitMeta.RawMessage);
        }
        var wrapped = message as IWrappedMessage;
        return base.PrepareMessage(wrapped?.Wrappee);
    }

    protected async override Task SendToChannel(IMessage message)
    {
        if (rabbitAdapter.Channel == null)
        {
            return;
        }

        var routingKey = message.Context[RabbitMessageContext.Type]?.ToString().ToLower();
        byte[] preparedMessage = PrepareMessage(message);

        BasicProperties props = new()
        {
            Type = message.Context[RabbitMessageContext.Type]?.ToString(),
            ContentType = "application/json",
            Headers = message.Context
        };

        if (routingKey == null || string.IsNullOrEmpty(options.ExchangeName))
        {
            return;
        }

        await rabbitAdapter.Channel.BasicPublishAsync(
            exchange: options.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: preparedMessage);
    }
}

