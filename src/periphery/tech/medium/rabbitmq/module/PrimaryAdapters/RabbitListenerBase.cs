namespace Tmobis.Medium.RabbitMQ.PrimaryAdapters;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Repositories;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.RabbitMQ.Client;

public abstract class RabbitListenerBase(
    IRabbitAdapter client,
    IRepositoryFactory repositoryFactory,
    ILogger<RabbitListenerBase> logger,
    IOptions<RabbitListenerOptions> options) : IHostedMessageListener
{
    protected virtual List<AsyncEventingBasicConsumer> Consumers { get; } = [];
    
    private readonly RabbitListenerOptions options = options.Value;

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Enabled)
        {
            return;
        }

        // TODO: Найти другой способ RetryPolicy
        while (true)
        {
            try
            {
                await client.Connect();
                await BindConsumer();
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при создании обработчика RabbitMQ");
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(options.ListenerConnectionRetryDelay, cancellationToken);
            }
        }
    }

    protected virtual async Task BindConsumer()
    {
        await client.CreateChannel();

        var consumer = new AsyncEventingBasicConsumer(client.Channel);
        consumer.ReceivedAsync +=
            async (_, ea) => await OnMessageReceivedAsync(client.Channel, ea);
        Consumers.Add(consumer);
    }

    protected virtual async Task OnMessageReceivedAsync(
        IChannel channel, BasicDeliverEventArgs @event)
    {
        try
        {
            if (channel == null)
            {
                return;
            }

            var contentType = @event.BasicProperties.ContentType;
            if (contentType == null || !contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
            {
                await channel.BasicNackAsync(@event.DeliveryTag, multiple: false, requeue: false);
                logger.LogWarning("Unsupported content type {ContentType}", contentType);
                return;
            }

            var originalMessage = ParseMessage(@event);
            var prepared = PrepareMessage(originalMessage);

            using var scope = repositoryFactory.MakeRepository();
            var dispatcher = scope.Scope.ServiceProvider.GetRequiredService<IMessageDispatcher>();
            await dispatcher.Send(prepared);
            await scope.UnitOfWork.Commit();

            await channel.BasicAckAsync(@event.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message");
            await channel.BasicNackAsync(@event.DeliveryTag, multiple: false, requeue: false);
        }
    }

    protected abstract IMessage ParseMessage(BasicDeliverEventArgs @event);

    protected virtual IMessage PrepareMessage(IMessage originalMessage)
    {
        return originalMessage;
    }

    public void Dispose()
    {
        client.Dispose();
        GC.SuppressFinalize(this);
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!options.Enabled)
        {
            Dispose();
            return;
        }

        if (client.Channels == null
            || Consumers == null
            || client.Channels.Count == 0
            || Consumers.Count == 0)
        {
            await client.Disconnect();
            Dispose();
            return;
        }

        try
        {
            foreach (var consumer in Consumers)
            {
                foreach (var consumerTag in consumer.ConsumerTags)
                {
                    await consumer.Channel
                        .BasicCancelAsync(consumerTag, false, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during consumer cancellation");
        }

        await client.Disconnect();
        Dispose();
    }
}
