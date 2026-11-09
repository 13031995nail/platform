namespace Tmobis.Medium.Retried.RabbitMQ.Service.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Delayed;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Medium.Retried.Models;
using Tmobis.Core.Modules;
using Tmobis.Jobs.Scheduled;
using Tmobis.Medium.Delayed.InRepo;
using Tmobis.Medium.Delayed.InRepo.Module;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.RabbitMQ.SecondaryAdapters;
using Tmobis.Medium.Retried.WithDelay;
using Tmobis.Medium.Retried.WithDelay.Module;

/// <summary>
/// Модуль сервиса повторной отправки сообщений через RabbitMQ DLQ.
/// </summary>
[ModuleRequirement([typeof(RabbitAdapterModule), typeof(DelayedMessageCreatedRepositoryProducerModule),
  typeof(DelayedMessageNowConsumerModule), typeof(DelayedMessageTickConsumerModule), typeof(RetriedMessageConsumerModule), typeof(DLQRabbitListenerModule)])]
public class RabbitMQDLQModule : ModuleBase
{
  public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddProducerMessage<DelayedMessageCreated>("DelayedMessageCreatedRepositoryProducer");
  }
}

