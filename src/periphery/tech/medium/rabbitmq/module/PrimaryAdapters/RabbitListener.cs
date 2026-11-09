namespace Tmobis.Medium.RabbitMQ.PrimaryAdapters;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Repositories;
using global::RabbitMQ.Client.Events;
using global::RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tmobis.Core.Medium.Mappers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.RabbitMQ.Client;

public class RabbitListener(
        IRabbitAdapter client,
        IRepositoryFactory repositoryFactory,
        ILogger<RabbitListener> logger,
        IOptions<RabbitListenerOptions> options,
        IMessageTypeBinder binder,
        IMessageListRegister messageListRegister)
        : RabbitListenerBase(client, repositoryFactory, logger, options)
{
    private readonly IRabbitAdapter client = client;
    private readonly RabbitListenerOptions options = options.Value;
    private readonly IReadOnlyList<Type> listenedMessageList = messageListRegister.GetListenedMessageList("RabbitMQ");

    protected override async Task BindConsumer()
{
    foreach (Type type in listenedMessageList)
    {
        string? queueName = type.FullName?.ToLower();

        if (string.IsNullOrEmpty(queueName))
        {
            logger.LogWarning("У типа сообщения {Type} не существует полного имени. " +
                "Создание очереди пропущено", type);
            continue;
        }

            // for (int idx = 0; idx < options.ThreadsPerMessageType; idx++)
            // {
            //     var consumer = new AsyncEventingBasicConsumer(client.Channel);
            //     consumer.ReceivedAsync +=
            //         async (_, ea) => await OnMessageReceivedAsync(client.Channel, ea);
            //     await client.Channel.BasicConsumeAsync(
            //         queueName, false, Guid.NewGuid().ToString(),
            //         true, false, null, consumer);
            //     Consumers.Add(consumer);
            // }

        for (int idx = 0; idx < options.ThreadsPerMessageType; idx++)
        {
            var channel = await client.CreateChannel();

            // ВМЕСТО создания очереди - используем пассивное объявление
            try
            {
                await channel.QueueDeclarePassiveAsync(queueName);
            }
            catch (OperationInterruptedException ex) when (ex.ShutdownReason.ReplyCode == 404)
            {
                // Если очереди нет - создаем с DLX
                if (!string.IsNullOrEmpty(options.DlxExchangeName))
                {
                    await client.CreateQueueWithDLX(channel, queueName, options.DlxExchangeName);
                }
                else
                {
                    await client.CreateQueue(channel, queueName);
                }
            }

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync +=
                async (_, ea) => await OnMessageReceivedAsync(channel, ea);
            await channel.BasicConsumeAsync(
                queueName, false, Guid.NewGuid().ToString(),
                true, false, null, consumer);
            Consumers.Add(consumer);
        }

            // for (int idx = 0; idx < options.ListenThreads; idx++)
            // {
            //     channel ??= await client.CreateChannel();
            //     var consumer = new AsyncEventingBasicConsumer(channel);
            //     var channelRef = channel;
            //     consumer.ReceivedAsync +=
            //         async (_, ea) => await OnMessageReceivedAsync(channelRef, ea);
            //     channel.BasicConsumeAsync(
            //         queueName, false, Guid.NewGuid().ToString(),
            //         true, false, null, consumer);
            //     Consumers.Add(consumer);
            //     channel = null;
            // }
    }
}

    protected override IMessage ParseMessage(BasicDeliverEventArgs @event)
    {
        if (listenedMessageList == null)
        {
            throw new InvalidOperationException("Список " + nameof(listenedMessageList)
              + " с типами сообщений не зарегестиррован.");
        }
        Type? messageType = listenedMessageList
            .FirstOrDefault(type =>
                type.FullName != null
                && type.FullName.Equals(@event.BasicProperties.Type, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Указанный тип сообщения " + @event.BasicProperties.Type + " не зарегестрирован.");

        string body = Encoding.UTF8.GetString(@event.Body.ToArray());
        JsonSerializerSettings settings = new() { SerializationBinder = binder };

        if (JsonConvert.DeserializeObject(body, messageType, settings) is not IMessage message)
        {
            throw new InvalidOperationException("Ошибка десериализации сообщения к типу " + messageType);
        }

        if (@event.BasicProperties?.Headers?.ContainsKey("x-death") ?? false)
        {
            var xDeath = @event.BasicProperties?.Headers?["x-death"] as List<IDictionary<string, Object>>;
            var xDeathItem = xDeath?.FirstOrDefault(item => item["exchange"].ToString() == options.SourceExchangeName);
            if (xDeathItem != null && xDeathItem["count"] != null)
            {
                // TODO: RetriedMessageContext.TryCount
                message.Context["TryCount"] = xDeathItem["count"];
            }
        }

        return message;
    }
}
