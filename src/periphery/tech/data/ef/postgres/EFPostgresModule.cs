namespace Tmobis.Data.EF.Postgres;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tmobis.Core.Modules;
using Tmobis.Core.DI;

/// <summary>
/// Модуль Entity Framework для PostgreSQL.
/// Использует PostgreSQL-реализацию <c>IDbContextConfigurator</c>.
/// </summary>
public class EFPostgresModule : ModuleBase
{
  public override void Register(
  IServiceCollection services,
  IConfiguration configuration,
  IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddOrReplaceTransient<IDbContextConfigurator, PostgresDbContextConfigurator>();
  }
}
