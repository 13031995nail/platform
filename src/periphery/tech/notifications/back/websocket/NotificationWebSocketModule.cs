namespace Tmobis.Notification.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;
using Tmobis.Notification.Sender;

/// <summary>
/// Модуль доставки уведомлений через WebSocket.
/// Содержит WebSocket provider.
/// </summary>
public class NotificationWebSocketModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        services.Configure<NotificationWebSocketOptions>(
            configuration.GetSection(NotificationWebSocketOptions.NotificationWebSocket));
        services.AddProducerMessage<SendNotification>("RabbitProducer");
        services.AddTransient<INotificationProvider, NotificationWebSocketProvider>();
    }
}
