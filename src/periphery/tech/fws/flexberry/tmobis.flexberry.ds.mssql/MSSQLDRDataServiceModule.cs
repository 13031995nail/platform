namespace Tmobis.Frameworks.Flexberry.DataServices.MSSQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ICSSoft.STORMNET.Business;
using Tmobis.Core.Modules;
using Tmobis.Core.DI;
using Tmobis.Frameworks.Flexberry.Modules;

/// <summary>
/// Модуль Flexberry DataService для Microsoft SQL Server в DR-режиме.
/// </summary>
[ModuleRequirement([typeof(FlexberryModule)])]
public sealed class MSSQLDRDataServiceModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddKeyedSingleton<
          IDataService, DRDataService>("Template");
    }
}
