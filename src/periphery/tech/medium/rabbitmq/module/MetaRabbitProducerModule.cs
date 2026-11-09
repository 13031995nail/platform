namespace Tmobis.Medium.RabbitMQ;

using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.RabbitMQ.SecondaryAdapters;

/// <summary>
/// Модуль вторичного адаптера метасообщений в RabbitMQ.
/// Использует RabbitMQ как реализацию общего <c>IMessageProducer</c>.
/// </summary>
[ModuleRequirement([typeof(RabbitAdapterModule)])]
public sealed class MetaRabbitProdcuerModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IMessageProducer, MetaRabbitProducer>();
    services.Configure<RabbitProducerOptions>(configuration.GetSection(RabbitProducerOptions.RabbitProducer));
  }
}
