namespace Tmobis.Frameworks.Flexberry.DataServices.Combined;
using Tmobis.Frameworks.Flexberry.DataServices.Postgres;
using Tmobis.Frameworks.Flexberry.DataServices.MSSQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tmobis.Data.DB.Postgres;
using Tmobis.Data.DB.MSSQL;
using Tmobis.Core.Modules;
using Tmobis.Core.DI;

/// <summary>
/// Модуль выбора Flexberry DataService по строке подключения.
/// Использует PostgreSQL или Microsoft SQL Server provider в зависимости от конфигурации.
/// </summary>
[ModuleRequirement([typeof(MSSQLDataServiceModule), typeof(PostgresDataServiceModule)])]
public class CombinedDataServiceModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    RegisterDataService(services, configuration, modules);
  }

  protected virtual void RegisterDataService(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    var connString = configuration.GetDefaultConnectionString();
    if (string.IsNullOrEmpty(connString))
    {
      throw new InvalidOperationException(
        "Строка подключения по умолчанию не найдена");
    }

    // TODO: Обобщить
    var isPostgres = PostgresConnectionStringValidator
      .IsValidConnectionString(connString);
    if (isPostgres)
    {
      // TODO: Изменить способ регистрации?
      new PostgresDataServiceModule().Register(services, configuration, modules);
      return;
    }

    var isMssql = MSSQLConnectionStringValidator
      .IsValidConnectionString(connString);
    if (isMssql)
    {
      // TODO: Изменить способ регистрации?
      new MSSQLDataServiceModule().Register(services, configuration, modules);
      return;
    }
  }
}
