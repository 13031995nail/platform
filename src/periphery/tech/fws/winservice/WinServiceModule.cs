namespace Tmobis.WinService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль запуска приложения как Windows Service.
/// </summary>
public sealed class WinServiceModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddWindowsService();
  }
}
