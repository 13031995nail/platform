namespace Tmobis.Frameworks.Flexberry.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewPlatform.Flexberry.Security;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Passwords;

/// <summary>
/// Модуль legacy SHA-1 хеширования паролей Flexberry.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule)])]
public class FlexberrySha1Module : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddOrReplaceTransient<Tmobis.Core.Passwords.IPasswordHasher, FlexberrySha1PasswordHasher>();
    services.AddOrReplaceSingleton<NewPlatform.Flexberry.Security.IPasswordHasher, FlexberrySha1PasswordHasher>();
  }
}
