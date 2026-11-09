namespace Tmobis.Medium.Delayed.InRepo.Module;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль персистентного хранения отложенных сообщений.
/// Сохраняет созданные delayed messages для последующей обработки.
/// </summary>
public sealed class DelayedMessageCreatedRepositoryProducerModule : ModuleBase, IProducerModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<DelayedMessageCreatedRepositoryProducer>();
  }
}
