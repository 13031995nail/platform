namespace Tmobis.Metrics;

using System.Diagnostics.Metrics;

public abstract class MetricsProvider
{
    protected virtual Meter Meter { get; }

    protected abstract string MeterName { get; }

    public MetricsProvider(IMeterFactory meterFactory)
    {
        Meter = meterFactory.Create(MeterName);
    }
}
