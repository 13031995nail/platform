namespace Tmobis.Metrics.Prometheus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tmobis.Core.Modules;
using global::OpenTelemetry.Metrics;
using Tmobis.Frameworks.Web.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Tmobis.Metrics.OpenTelemetry;
using global::OpenTelemetry.Resources;

/// <summary>
/// Модуль Prometheus exporter для метрик приложения.
/// Публикует HTTP endpoint для scraping.
/// </summary>
[ModuleRequirement(typeof(OpenTelemetryModule))]
public sealed class PrometheusMetricsModule : WebFrameworkModule
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddOpenTelemetry()
        .WithMetrics(builder => builder
            .AddPrometheusExporter());
    }

    public override void ConfigureWebPipeline(
        IApplicationBuilder app,
        IConfiguration configuration,
        IHostEnvironment env,
        IModuleRegister modules)
    {
        base.ConfigureWebPipeline(app, configuration, env, modules);
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}
