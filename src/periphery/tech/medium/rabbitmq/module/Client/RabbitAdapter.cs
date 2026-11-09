namespace Tmobis.Medium.RabbitMQ.Client;

using System;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class RabbitAdapter : IRabbitAdapter
{
    public virtual IChannel? Channel => Channels?.FirstOrDefault();

    public virtual List<IChannel> Channels { get; protected set; } = [];

    private readonly ConnectionFactory connectionFactory;

    private IConnection? connection;

    private bool disposed;

    private readonly ILogger<RabbitAdapter> logger;

    private readonly RabbitOptions options;

    public RabbitAdapter(
        IOptions<RabbitOptions> options,
        ILogger<RabbitAdapter> logger)
    {
        this.logger = logger;
        ArgumentNullException.ThrowIfNull(options);

        this.options = options.Value;
        connectionFactory = new ConnectionFactory()
        {
            HostName = this.options.Host,
            UserName = this.options.User,
            Password = this.options.Password,
            VirtualHost = this.options.VirtualHost ?? "/",
            RequestedHeartbeat = this.options.HeartbeatTimeout,
            RequestedConnectionTimeout = this.options.ConnectionTimeout,
        };
    }

    public async virtual Task CreateExchange(
        IChannel channel, string exchangeName, string type)
    {
        if (channel == null)
        {
            logger.LogError("Канал не создан. Создание обмена невозможно.");
            return;
        }

        await channel
            .ExchangeDeclareAsync(
                exchangeName, type, durable: true, autoDelete: false);
    }

    public async virtual Task CreateQueue(
        IChannel channel, string queueName)
    {
        if (channel == null)
        {
            logger.LogError("Канал не создан. Создание очереди невозможно.");
            return;
        }

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });
    }

    public async virtual Task CreateQueueWithDLX(
        IChannel channel, string queueName, string dlxExchange)
    {
        if (channel == null)
        {
            logger.LogError("Канал не создан. Создание очереди невозможно.");
            return;
        }

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" }, { "x-dead-letter-exchange", dlxExchange }});
  }

    public async virtual Task BindQueue(
        IChannel channel, string queueName, string exchangeName)
    {
        if (channel == null)
        {
            logger.LogError("Канал не создан. Бинд очереди невозможен.");
            return;
        }

        await channel.QueueBindAsync(queueName, exchangeName, queueName);
    }

    public virtual async Task Connect()
    {
        try
        {
            foreach (var channel in Channels)
            {
                if (channel != null && channel.IsOpen)
                {
                    try
                    {
                        await channel.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Ошибка при переподключении к RabbitMQ");
                    }
                }
            }

            Channels.Clear();

            connection ??= await connectionFactory.CreateConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectFailureException("Ошибка при создании подключения к RabbitMQ", ex);
        }
    }

    public virtual async Task Disconnect()
    {
        try
        {
            if (Channels != null)
            {
                foreach (var channel in Channels)
                {
                    await channel.CloseAsync();
                }
            }

            if (connection != null)
            {
                await connection.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw new ConnectFailureException("Ошибка при закрытии подключения к RabbitMQ", ex);
        }
    }

    public virtual async Task<IChannel> CreateChannel()
    {
        if (connection == null)
        {
            throw new InvalidOperationException("Не подключен.");
        }

        var channel = await connection.CreateChannelAsync();
        await channel.BasicQosAsync(0, options.PrefetchCount, false);
        Channels.Add(channel);
        return channel;
    }

    public virtual async Task DestroyChannel(IChannel channel)
    {
        Channels.Remove(channel);
        channel.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                if (Channels != null)
                {
                    foreach (var channel in Channels)
                    {
                        channel.Dispose();
                    }
                }
                connection?.Dispose();
            }
            disposed = true;
        }
    }
}
