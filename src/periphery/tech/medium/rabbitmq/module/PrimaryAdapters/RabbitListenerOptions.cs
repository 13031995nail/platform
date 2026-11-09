namespace Tmobis.Medium.RabbitMQ.PrimaryAdapters;

public class RabbitListenerOptions
{
    public const string RabbitListener = "RabbitListener";

    public bool Enabled { get; set; } = true;

    public string? SourceExchangeName { get; set; }

    public string? DlxExchangeName { get; set; }

    public int ThreadsPerMessageType { get; set; } = 1;

    public TimeSpan ListenerConnectionRetryDelay { get; set; } = new(0, 1, 0);
}
