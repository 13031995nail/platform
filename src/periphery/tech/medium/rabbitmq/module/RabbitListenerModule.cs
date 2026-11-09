namespace Tmobis.Medium.RabbitMQ;

using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Medium.RabbitMQ.PrimaryAdapters;
using Tmobis.Core.Medium.Modules;

/// <summary>
/// Модуль первичного адаптера RabbitMQ.
/// Использует hosted message-listener инфраструктуру для приёма сообщений.
/// </summary>
[ModuleRequirement([
  typeof(RabbitAdapterModule), typeof(MessageListenerWorkerModule)])]
public sealed class RabbitListenerModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IHostedMessageListener, RabbitListener>();
    services.Configure<RabbitListenerOptions>(configuration.GetSection(RabbitListenerOptions.RabbitListener));
  }
}
