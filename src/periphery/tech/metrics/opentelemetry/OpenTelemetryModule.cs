namespace Tmobis.Metrics.OpenTelemetry;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tmobis.Core.Modules;
using global::OpenTelemetry.Metrics;
using Microsoft.Extensions.Options;

/// <summary>
/// Модуль сбора метрик через OpenTelemetry.
/// Использует список Meter-ов из общих настроек метрик.
/// </summary>
[ModuleRequirement(typeof(MetricsModule))]
public sealed class OpenTelemetryModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        var options = new MetricsOptions();
        configuration
        .GetSection(MetricsOptions.Metrics)
        .Bind(options);

        services.AddOpenTelemetry()
        .WithMetrics(builder =>
        {
            options.Listened.ForEach(meter => builder.AddMeter(meter));
        });
    }

    public override void Configure(
      IServiceProvider services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Configure(services, configuration, modules);
    }
}
