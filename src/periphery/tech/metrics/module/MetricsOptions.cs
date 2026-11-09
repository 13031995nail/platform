namespace Tmobis.Metrics;

public record MetricsOptions
{
    public const string Metrics = "Metrics";

    public List<string> Listened { get; set; } = [];
}
