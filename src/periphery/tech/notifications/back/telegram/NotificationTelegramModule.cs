namespace Tmobis.Notification.Telegram;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Messengers.Telegram;
using Tmobis.Notification.Sender;

/// <summary>
/// Модуль доставки уведомлений через Telegram.
/// Содержит Telegram provider и настройки канала.
/// </summary>
[ModuleRequirement(
    typeof(NotificationModule),
    typeof(NotificationSenderModule),
    typeof(TelegramModule))]
public sealed class NotificationTelegramModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<NotificationTelegramOptions>(
            configuration.GetSection(
                NotificationTelegramOptions.SectionName));
        services.AddTransient<
            INotificationProvider,
            NotificationTelegramProvider>();
    }
}
