namespace Tmobis.Medium.RabbitMQ.SecondaryAdapters;

using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tmobis.Medium.RabbitMQ.Client;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Models;
using Microsoft.Extensions.Options;

public abstract class RabbitProducerBase(
    IRabbitAdapter rabbitAdapter,
    ILogger<RabbitProducerBase> logger,
    IOptions<RabbitProducerOptions> options) : IMessageProducer
{
    private readonly ILogger<RabbitProducerBase> logger = logger;
    private readonly RabbitProducerOptions options = options.Value;

    public virtual Task<IMessageResult> SendWithResult (IMessage message) => throw new InvalidOperationException();

    public virtual Task Publish (IMessage message) => throw new NotImplementedException();

    public async virtual Task Send(IMessage message)
    {
        try
        {
            await rabbitAdapter.Connect();
            await rabbitAdapter.CreateChannel();
            await InitModel(message);
            await SendToChannel(message);
            await rabbitAdapter.Disconnect();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при отправке сообщения в RabbitMQ");
        }
    }

    protected async virtual Task InitModel(IMessage message)
    {
        var exchangeName = options.ExchangeName;
        var exchangeType = options.ExchangeType;

        if (string.IsNullOrEmpty(exchangeName) || string.IsNullOrEmpty(exchangeType))
        {
            logger.LogWarning("В настройки продюсера не переданы тип или имя обмена");
            return;
        }
        await rabbitAdapter.CreateExchange(rabbitAdapter.Channel, exchangeName, exchangeType);
    }

    protected abstract Task SendToChannel(IMessage message);

    protected virtual byte[] PrepareMessage(IMessage? message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var messageJson = JsonConvert.SerializeObject(message);
        return Encoding.UTF8.GetBytes(messageJson);
    }
}
