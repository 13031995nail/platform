namespace Tmobis.Notification;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Notification.Messages;

/// <summary>
/// Модуль подсистемы уведомлений.
/// Содержит сущности, message/consumer-контракты и команды работы с уведомлениями.
/// </summary>
[ModuleRequirement(typeof(BusinessCoreModule))]
public class NotificationModule
    : ModuleBase, IEntityModule, IMessageModule, IConsumerModule
{
    /// <inheritdoc/>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddListenedMessage<GetNotifications>("WebApi");
        services.AddListenedMessage<GetUnreadNotificationCount>("WebApi");
    }

    public List<Assembly> GetAdditionalModelAssemblies() => [];
}
