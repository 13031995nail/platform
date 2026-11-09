namespace Tmobis.Auth.Windows.Modules;

using Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Auth.JwtAuth.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль Windows-аутентификации.
/// Использует <c>WindowsAuthController</c> в JWT auth-сценарии.
/// </summary>
[ModuleRequirement(typeof(JwtAuthModule))]
public sealed class WindowsAuthModule
: ModuleBase
{
  public override void Register(
  IServiceCollection services,
  IConfiguration configuration,
  IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<WindowsAuthController>();
  }
}
