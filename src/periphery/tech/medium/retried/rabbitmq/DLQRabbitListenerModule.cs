namespace Tmobis.Medium.Retried.RabbitMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.Retried.RabbitMQ.PrimaryAdapters;

/// <summary>
/// Модуль первичного адаптера RabbitMQ для dead-letter очереди.
/// Используется подсистемой повторной обработки сообщений.
/// </summary>
[ModuleRequirement([
typeof(RabbitAdapterModule), typeof(MessageListenerWorkerModule)])]
public sealed class DLQRabbitListenerModule : ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IHostedMessageListener, DLQRabbitListener>();
    services.Configure<DLQRabbitListenerOptions>(configuration.GetSection(DLQRabbitListenerOptions.DLQRabbitListener));
  }
}

