namespace Tmobis.Jobs.Scheduled;
using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Модуль конфигурации расписаний сообщений.
/// Содержит сопоставление message-типов с cron-выражениями.
/// </summary>
public sealed class MessageScheduleModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<ScheduledMessagesOptions>(
            configuration.GetSection(
              ScheduledMessagesOptions.ScheduledMessages));
    }
}
