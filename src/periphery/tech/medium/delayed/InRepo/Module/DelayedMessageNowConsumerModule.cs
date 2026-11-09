namespace Tmobis.Medium.Delayed.InRepo.Module;

using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.RabbitMQ.SecondaryAdapters;

/// <summary>
/// Модуль персистентного хранения отложенных сообщений.
/// Обработчик события немедленной отправки ранее отложенных сообщений.
/// </summary>
public sealed class DelayedMessageNowConsumerModule : ModuleBase, IConsumerModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IMessageProducer, MetaRabbitProducer>();
  }
}
