namespace Tmobis.Mobile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Mobile.Options;

/// <summary>
/// Модуль настроек мобильной подсистемы.
/// </summary>
public sealed class MobileModule : ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.Configure<MobileOptions>(
      configuration.GetSection(MobileOptions.Mobile));
  }
}
