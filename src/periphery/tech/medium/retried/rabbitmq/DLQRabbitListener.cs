namespace Tmobis.Medium.Retried.RabbitMQ;

using Microsoft.Extensions.Options;
using Tmobis.Medium.Retried.RabbitMQ.PrimaryAdapters;
using Tmobis.Medium.RabbitMQ.PrimaryAdapters;
using Tmobis.Medium.RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Medium.Producers;
using global::RabbitMQ.Client.Events;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Retried.Models;
using System.Text;
using Core.Repositories;
using Tmobis.Medium.RabbitMQ.Models;

public class DLQRabbitListener(
  IRabbitAdapter client,
  IRepositoryFactory repositoryFactory,
  ILogger<DLQRabbitListener> logger,
  IOptions<RabbitListenerOptions> rabbitOptions,
  IOptions<DLQRabbitListenerOptions> options) : MetaRabbitListener(client, repositoryFactory, logger, rabbitOptions)
{
  private readonly string queueName = options.Value.QueueName;
  private readonly IRabbitAdapter client = client;
  private readonly DLQRabbitListenerOptions options = options.Value;
  private readonly RabbitListenerOptions rabbitOptions = rabbitOptions.Value;

  protected override async Task BindConsumer()
  {
          await base.BindConsumer();
          await client.CreateQueue(client.Channel, queueName);
          if (client.Channel == null)
          {
              logger.LogError("Канал не создан. Бинд очереди невозможен.");
              return;
          }
          await client.Channel.QueueBindAsync(queueName, options.ExchangeName, options.DLXRoutingKey);
          var consumer = Consumers.Last();
          await client.Channel.BasicConsumeAsync(
              queueName, false, Guid.NewGuid().ToString(),
              true, false, null, consumer);
  }

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

    if (@event.BasicProperties?.Headers?.ContainsKey("x-death") ?? false)
    {
      var xDeathHeader = @event.BasicProperties.Headers["x-death"];
      if (xDeathHeader is List<object> xDeathDictList)
      {
        foreach (var item in xDeathDictList)
        {
          if (item is Dictionary<string, object> dict)
          {

            if (dict.ContainsKey("exchange") &&
                Encoding.UTF8.GetString((byte[])dict["exchange"]) == rabbitOptions.SourceExchangeName &&
                dict.ContainsKey("count"))
            {
              metaMessage.Context[RetriedMessageContext.TryCount] = dict["count"];
              metaMessage.Context[RetriedMessageContext.MaxTryCount] = options.RetriedMaxTryCount;
            }
          }
        }
      }
    }

    metaMessage.Context[RabbitMessageContext.Type] = @event.BasicProperties.Type;
    metaMessage.Context[RabbitMessageContext.RoutingKey] = @event.RoutingKey;

    return metaMessage;
  }

  protected override IMessage PrepareMessage(IMessage? message)
  {
    ArgumentNullException.ThrowIfNull(message);

    var retriedMessage = new RetriedMessageCreated
    {
      Wrappee = message
    };

    return retriedMessage;
  }
}

