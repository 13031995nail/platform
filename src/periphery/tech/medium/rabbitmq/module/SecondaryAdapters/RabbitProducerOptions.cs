namespace Tmobis.Medium.RabbitMQ.SecondaryAdapters;

public class RabbitProducerOptions
{
    public const string RabbitProducer = "RabbitProducer";

    public string? ExchangeName { get; set; }

    public string? ExchangeType { get; set; }

    public string? DlxExchangeName { get; set; }
}
