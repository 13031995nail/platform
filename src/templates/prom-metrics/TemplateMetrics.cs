namespace Tmobis.Prometheus.Template;

using Tmobis.Metrics;
using System.Diagnostics.Metrics;

public class TemplateMetrics : MetricsProvider, ITemplateMetrics
{
    public Counter<long> GitlabSecurityIssues { get; }

    protected override string MeterName => "Timepath.Code";

    public TemplateMetrics(IMeterFactory meterFactory) : base(meterFactory)
    {
        GitlabSecurityIssues = Meter
            .CreateCounter<long>("timepath.code.gitlab.security.issues");
    }
}
