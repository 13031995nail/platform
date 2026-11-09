namespace Tmobis.Prometheus.Template;

using System.Diagnostics.Metrics;

public interface ITemplateMetrics
{
    Counter<long> GitlabSecurityIssues { get; }
}
