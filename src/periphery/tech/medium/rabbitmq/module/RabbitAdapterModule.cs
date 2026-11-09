namespace Tmobis.Medium.RabbitMQ;

using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Medium.RabbitMQ.Client;

/// <summary>
/// Модуль базового адаптера RabbitMQ.
/// Содержит adapter подключения к RabbitMQ для listeners и producers.
/// </summary>
public sealed class RabbitAdapterModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IRabbitAdapter, RabbitAdapter>();
        services.Configure<RabbitOptions>(configuration.GetSection(RabbitOptions.Rabbit));
    }
}
