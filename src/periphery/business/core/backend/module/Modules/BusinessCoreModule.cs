namespace Tmobis.Business.Core.Modules;

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Options;
using Providers.Employees;
using Providers.Roles;
using Providers.Towns;
using Providers.Users;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Бизнес-модуль ядра бизнес-логики Платформы Т-Мобис.
/// Содержит базовые бизнес-сущности, сообщения и provider-ы пользователей, сотрудников, ролей и населённых пунктов.
/// </summary>
public sealed class BusinessCoreModule
  : ModuleBase, IEntityModule, IConsumerModule, IMessageModule
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.Configure<UserProviderOptions>(
      configuration.GetSection(UserProviderOptions.UserProvider));
    services.AddTransient<IUserProvider, UserProvider>();
    services.AddTransient<IEmployeeProvider, EmployeeProvider>();
    services.AddTransient<IRoleProvider, EmptyRoleProvider>();
    services.AddTransient<ICurrentRoleProvider, RoleProviderCurrentRoleProvider>();
    services.AddTransient<ITownProvider, TownProvider>();
    services.AddTransient<ICurrentUserProvider, EmptyCurrentUserProvider>();
    services.AddSingleton<ITownLimitResolver, TownLimitResolver>();
    services.AddScoped<ICurrentTownProvider, CurrentUserTownProvider>();
    services.AddMemoryCache();
  }
}
