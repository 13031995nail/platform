namespace Tmobis.Notification.Vk;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Messengers.Vk;
using Tmobis.Notification;
using Tmobis.Notification.Sender;

/// <summary>
/// Модуль доставки уведомлений через VK.
/// Содержит VK provider и настройки канала.
/// </summary>
[ModuleRequirement(
    typeof(NotificationModule),
    typeof(NotificationSenderModule),
    typeof(VkModule))]
public sealed class NotificationVkModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<NotificationVkOptions>(
            configuration.GetSection(NotificationVkOptions.SectionName));
        services.AddTransient<INotificationProvider, NotificationVkProvider>();
    }
}
