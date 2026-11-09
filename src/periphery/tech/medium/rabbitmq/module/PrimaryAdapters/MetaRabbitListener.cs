namespace Tmobis.Medium.RabbitMQ.PrimaryAdapters;

using System.Text;
using Core.Repositories;
using global::RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.RabbitMQ.Client;
using Tmobis.Medium.RabbitMQ.Models;

public abstract class MetaRabbitListener(
    IRabbitAdapter client,
    IRepositoryFactory repositoryFactory,
    ILogger<MetaRabbitListener> logger,
    IOptions<RabbitListenerOptions> options) : RabbitListenerBase(client, repositoryFactory, logger, options)
{
    protected override IMessage ParseMessage(BasicDeliverEventArgs @event)
    {
        var body = Encoding.UTF8.GetString(@event.Body.ToArray());
        var metaMessage = new RabbitMetaMessage { RawMessage = body };

        if (@event.BasicProperties.Headers != null)
        {
            foreach (var header in @event.BasicProperties.Headers)
            {
                metaMessage.Context[header.Key] = header.Value;
            }
        }

        metaMessage.Context[RabbitMessageContext.Type] = @event.BasicProperties.Type;
        metaMessage.Context[RabbitMessageContext.RoutingKey] = @event.RoutingKey;

        return metaMessage;
    }
}
