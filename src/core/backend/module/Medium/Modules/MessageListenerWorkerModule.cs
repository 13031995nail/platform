namespace Tmobis.Core.Medium.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Core.Medium.Listeners;

/// <summary>
/// Модуль фонового запуска и обслуживания <c>IHostedMessageListener</c>.
/// </summary>
public class MessageListenerWorkerModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddHostedService<MessageListenerWorker>();
  }
}
