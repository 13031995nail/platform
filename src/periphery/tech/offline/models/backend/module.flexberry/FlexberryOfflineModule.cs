namespace Tmobis.Offline.Flexberry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Offline.Converters;
using Tmobis.Offline.Flexberry.Converters;

/// <summary>
/// Модуль контрактов офлайн-режима на основе Flexberry.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule), typeof(OfflineModule)])]
public sealed class FlexberryOfflineModule : ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddOrReplaceTransient<
        IOfflineJsonContractResolverProvider,
        FlexberryOfflineJsonContractResolverProvider>();
  }
}
