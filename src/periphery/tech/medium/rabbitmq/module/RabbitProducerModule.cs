namespace Tmobis.Medium.RabbitMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Modules;
using Tmobis.Medium.RabbitMQ.SecondaryAdapters;

/// <summary>
/// Модуль вторичного адаптера RabbitMQ.
/// </summary>
[ModuleRequirement([typeof(RabbitAdapterModule)])]
public sealed class RabbitProducerModule : ModuleBase, IProducerModule
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<RabbitProducerOptions>(configuration.GetSection(RabbitProducerOptions.RabbitProducer));
        services.AddTransient<RabbitProducer>();
    }
}
