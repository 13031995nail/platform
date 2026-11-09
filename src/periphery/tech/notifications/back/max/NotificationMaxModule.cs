namespace Tmobis.Notification.Max;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Messengers.Max;
using Tmobis.Notification;
using Tmobis.Notification.Sender;

/// <summary>
/// Модуль доставки уведомлений через MAX.
/// Содержит MAX provider и настройки канала.
/// </summary>
[ModuleRequirement(
    typeof(NotificationModule),
    typeof(NotificationSenderModule),
    typeof(MaxModule))]
public class NotificationMaxModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<NotificationMaxOptions>(
            configuration.GetSection(
                NotificationMaxOptions.NotificationMax));
        services.AddTransient<
            INotificationProvider,
            NotificationMaxProvider>();
    }
}
