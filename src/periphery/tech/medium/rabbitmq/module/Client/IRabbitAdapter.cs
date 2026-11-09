namespace Tmobis.Medium.RabbitMQ.Client;

using System;
using global::RabbitMQ.Client;

public interface IRabbitAdapter : IDisposable
{
    public IChannel? Channel { get; }

    public List<IChannel> Channels { get; }

    public Task Connect();

    public Task Disconnect();

    public Task<IChannel> CreateChannel();

    public Task DestroyChannel(IChannel channel);

    public Task CreateExchange(IChannel channel, string exchangeName, string type);

    public Task CreateQueue(IChannel channel, string queueName);

    public Task CreateQueueWithDLX(IChannel channel, string queueName, string dlxExchange);

    public Task BindQueue(
        IChannel channel, string queueName, string exchangeName);
}
