namespace Tmobis.Prometheus.Template;

using Microsoft.Extensions.Configuration;
using Tmobis.Core.Modules;

public sealed class TemplateMetricsModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddSingleton<ITemplateMetrics, TemplateMetrics>();
    }
}
