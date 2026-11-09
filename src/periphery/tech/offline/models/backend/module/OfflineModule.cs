namespace Tmobis.Offline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Offline.Converters;
using Tmobis.Offline.Options;

/// <summary>
/// Модуль настроек офлайн-режима мобильного приложения.
/// </summary>
public sealed class OfflineModule : ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.Configure<OfflineOptions>(
      configuration.GetSection(OfflineOptions.Offline));
    services.AddTransient<
        IOfflineJsonContractResolverProvider,
        OfflineJsonContractResolverProvider>();
  }
}
