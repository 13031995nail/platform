#pragma warning disable SA1200 // Using directives should be placed correctlyusing Tmobis.Backend.Modules;
using System.Diagnostics.Metrics;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Metrics;
using Tmobis.Metrics.OpenTelemetry;
using Tmobis.Metrics.Prometheus;
using Tmobis.Prometheus.Template;
#pragma warning restore SA1200 // Using directives should be placed correctly

var modules = new ModuleRegisterBuilder([
    typeof(WebFrameworkModule),
    typeof(MetricsModule),
    typeof(OpenTelemetryModule),
    typeof(PrometheusMetricsModule),
    typeof(TemplateMetricsModule),
]);

var builder = new WebAppBuilder(modules, args);
var app = builder.Build();

var service = app.Services.GetService<ITemplateMetrics>();
service.GitlabSecurityIssues.Add(1);

await app.RunAsync();