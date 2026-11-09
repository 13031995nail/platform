namespace Tmobis.Offline.UseCases.Flexberry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Offline.Flexberry;
using Tmobis.Offline.Flexberry.Messages;
using Tmobis.Offline.UseCases.Flexberry.Consumers;
using Tmobis.Utils.Lzw;

/// <summary>
/// Модуль бизнес-обработки запросов офлайн sync/backup на основе Flexberry.
/// </summary>
[ModuleRequirement([
  typeof(BusinessCoreModule),
  typeof(FlexberryModule),
  typeof(LzwModule),
  typeof(FlexberryOfflineModule)])]
public sealed class FlexberryOfflineUseCasesModule : ModuleBase, IConsumerModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
  }
}
