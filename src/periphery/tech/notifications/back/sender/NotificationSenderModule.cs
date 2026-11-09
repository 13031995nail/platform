namespace Tmobis.Notification.Sender;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль отправки уведомлений.
/// Принимает <c>CreateNotification</c> через RabbitMQ и WebApi.
/// </summary>
public class NotificationSenderModule : ModuleBase, IConsumerModule
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddListenedMessage<CreateNotification>("RabbitMQ");
        services.AddListenedMessage<CreateNotification>("WebApi");
    }
}
