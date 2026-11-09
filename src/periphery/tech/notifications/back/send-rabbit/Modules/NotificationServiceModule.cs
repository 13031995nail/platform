namespace Tmobis.Notification.Send.Rabbit.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Jobs.Scheduled;
using Tmobis.Notification.Send.Rabbit.Consumers;
using Tmobis.Notification.Send.Rabbit.Models.Messages;

/// <summary>
/// Модуль плановой отправки уведомлений через RabbitMQ.
/// Содержит scheduled message и consumer отправки уведомлений.
/// </summary>
public class NotificationServiceModule : ModuleBase, IMessageModule, IScheduledMessagesModule
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IMessageConsumer<SendRabbitNotification>, SendRabbitNotificationConsumer>();
        services.AddProducerMessage<CreateNotification>("RabbitProducer");
    }
    public List<Type> GetScheduledMessages() =>
    [typeof(SendRabbitNotification)];

}
