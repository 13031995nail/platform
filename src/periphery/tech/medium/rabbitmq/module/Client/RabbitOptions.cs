namespace Tmobis.Medium.RabbitMQ.Client;

public class RabbitOptions
{
    public const string Rabbit = "Rabbit";

    public string? Host { get; set; }

    public string? User { get; set; }

    public string? Password { get; set; }

    public string? VirtualHost { get; set; }

    public ushort PrefetchCount { get; set; } = 1;

    public TimeSpan HeartbeatTimeout { get; set; } = TimeSpan.FromMinutes(2);

    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromMinutes(2);
}
