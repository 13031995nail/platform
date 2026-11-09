namespace Tmobis.Metrics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль метрик приложения.
/// Содержит общие metrics services и список Meter-ов для exporter-модулей.
/// </summary>
public sealed class MetricsModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddMetrics();
        services.Configure<MetricsOptions>(
            configuration.GetSection(MetricsOptions.Metrics));
    }
}
