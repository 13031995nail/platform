namespace Tmobis.Frameworks.Flexberry.Web.Modules;

using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Controllers.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewPlatform.Flexberry;
using NewPlatform.Flexberry.Reports.ExportToExcel;
using Tmobis.Business.Core.Flexberry.Modules;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;
using Tmobis.Core.Repositories;
using Tmobis.Core.Entities;
using Tmobis.Frameworks.Flexberry.Repositories;
using Tmobis.Frameworks.Flexberry.Web.Repositories;
using Tmobis.Frameworks.Flexberry.DataServices;
using Tmobis.Frameworks.Flexberry.TownLimit;
using Tmobis.Frameworks.Flexberry.Web.TownLimit;
using Tmobis.Frameworks.Flexberry.Web.Users;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Files.PrimaryAdapter.Flexberry;

/// <summary>
/// Модуль web-инфраструктуры Flexberry.
/// Содержит Excel export, текущего пользователя, ограничения данных и backend DataService.
/// </summary>
[ModuleRequirement([
  typeof(FlexberryBusinessCoreModule),
  typeof(FlexberryFileAdapterModule),
  typeof(WebFrameworkModule)])]
public class FlexberryWebModule : ModuleBase, IWebPipelineModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    RegisterServices(services, configuration, modules);
  }

  protected virtual void RegisterServices(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services.AddTransient<IExportService, ExportExcelODataService>();
    services.AddTransient<IUserSpecificLimiter, UserSpecificLimiter>();
    services.AddOrReplaceSingleton<
      NewPlatform.Flexberry.ORM.CurrentUserService.ICurrentUser,
      FlexberryCurrentUser>();
    RegisterDataService(services, configuration);
    RegisterTownLimitedRepository(services);
  }

  protected virtual void RegisterTownLimitedRepository(IServiceCollection services)
  {
    // Keyed-репозиторий не должен делить DataObjectCache с unrestricted.
    services.AddKeyedScoped<FlexberryRepositoryContext>(RepositoryKeys.TownLimited);
    services.AddKeyedTransient(
      typeof(IEntityRepository<,>),
      RepositoryKeys.TownLimited,
      typeof(FlexberryTownLimitedEntityRepository<,>));

    // Backend IDataService уже содержит WithSecurity + WithTownLimit.
    services.AddKeyedScoped<FlexberryRepository>(RepositoryKeys.TownLimited,
      (provider, _) => new FlexberryRepository(
        provider,
        provider.GetRequiredService<IEntityMaterializer>(),
        provider.GetRequiredKeyedService<IDataService>("Backend"),
        provider.GetRequiredKeyedService<FlexberryRepositoryContext>(RepositoryKeys.TownLimited),
        RepositoryKeys.TownLimited));

    services.AddKeyedScoped<IReadRepository>(RepositoryKeys.TownLimited,
      (provider, _) => provider.GetRequiredKeyedService<FlexberryRepository>(RepositoryKeys.TownLimited));
    services.AddKeyedScoped<IWriteRepository>(RepositoryKeys.TownLimited,
      (provider, _) => provider.GetRequiredKeyedService<FlexberryRepository>(RepositoryKeys.TownLimited));
    services.AddKeyedScoped<IRepository>(RepositoryKeys.TownLimited,
      (provider, _) => provider.GetRequiredKeyedService<FlexberryRepository>(RepositoryKeys.TownLimited));
    services.AddKeyedScoped<IUnitOfWork>(RepositoryKeys.TownLimited,
      (provider, _) => provider.GetRequiredKeyedService<FlexberryRepository>(RepositoryKeys.TownLimited));
  }

  protected virtual void RegisterDataService(
    IServiceCollection services,
    IConfiguration configuration)
  {
    services.AddKeyedScoped(
      "Backend",
      (reg, key) =>
      {
          var userSpecificLimiter = reg.GetRequiredService<IUserSpecificLimiter>();
          var factory = reg.GetRequiredService<IDataServiceFactory>();
          return factory
            .Create()
            .WithSecurity()
            .WithTownLimit(userSpecificLimiter)
            .DataService
            ?? throw new InvalidOperationException(
              "Не удалось построить DataService");
      });
  }

  public virtual void ConfigureWebPipeline(
     IApplicationBuilder app,
     IConfiguration configuration,
     IHostEnvironment env,
     IModuleRegister modules)
    {
        app.UseMvc(routes =>
        {
            routes.MapLogsRoute();
        });
    }
}
