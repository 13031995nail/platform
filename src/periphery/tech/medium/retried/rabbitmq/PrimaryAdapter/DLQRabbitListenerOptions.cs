namespace Tmobis.Medium.Retried.RabbitMQ.PrimaryAdapters;

public class DLQRabbitListenerOptions
{
  public const string DLQRabbitListener = "DLQRabbitListener";

  public string QueueName { get; set; }

  public string? ExchangeName { get; set; }

  public int RetriedMaxTryCount { get; set; }

  public int DelayedTime { get; set; }

  public string? DLXRoutingKey { get; set; }

}
