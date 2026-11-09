namespace Tmobis.Business.Core.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Providers.Roles;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль определения текущей роли пользователя по claims.
/// </summary>
[ModuleRequirement([typeof(BusinessCoreModule)])]
public sealed class ClaimsRoleProviderModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddOrReplaceTransient<ICurrentRoleProvider, ClaimsRoleProvider>();
  }
}
