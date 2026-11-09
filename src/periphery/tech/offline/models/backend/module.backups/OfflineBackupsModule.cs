namespace Tmobis.Offline.Backups;

using Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Jobs.Scheduled;
using Tmobis.Offline.Backups.Consumers;
using Tmobis.Offline.Backups.Models;
using Tmobis.Offline.Backups.Options;
using Tmobis.Offline.Converters;

/// <summary>
/// Модуль создания и обслуживания офлайн бэкапов.
/// Содержит контракты, настройки, JSON contract resolver и планируемое сообщение <c>MakeOfflineBackup</c>.
/// </summary>
[ModuleRequirement([
  typeof(BusinessCoreModule),
  typeof(MessageScheduleModule),
  typeof(OfflineModule)])]
public sealed class OfflineBackupsModule : ModuleBase, IMessageModule, IScheduledMessagesModule, IConsumerModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.Configure<OfflineBackupOptions>(configuration.GetSection(OfflineBackupOptions.OfflineBackup));
    services.AddTransient<IOfflineJsonContractResolverProvider, OfflineJsonContractResolverProvider>();
  }

  public List<Type> GetScheduledMessages() =>
    [typeof(MakeOfflineBackup)];
}
