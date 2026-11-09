namespace Tmobis.Notification.Mattermost;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Messengers.Mattermost;
using Tmobis.Notification;
using Tmobis.Notification.Sender;

/// <summary>
/// Модуль доставки уведомлений через Mattermost.
/// Содержит Mattermost provider и настройки канала.
/// </summary>
[ModuleRequirement(
    typeof(NotificationModule),
    typeof(NotificationSenderModule),
    typeof(MattermostModule))]
public sealed class NotificationMattermostModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<NotificationMattermostOptions>(
            configuration.GetSection(
                NotificationMattermostOptions.SectionName));
        services.AddTransient<
            INotificationProvider,
            NotificationMattermostProvider>();
    }
}
