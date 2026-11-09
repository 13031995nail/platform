namespace Tmobis.Frameworks.Flexberry.DataServices.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ICSSoft.STORMNET.Business;
using Tmobis.Core.Modules;
using Tmobis.Core.DI;
using Tmobis.Frameworks.Flexberry.Modules;

/// <summary>
/// Модуль Flexberry DataService для PostgreSQL.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule)])]
public sealed class PostgresDataServiceModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddKeyedSingleton<
            IDataService, PostgresDataService>("Template");
    }
}
