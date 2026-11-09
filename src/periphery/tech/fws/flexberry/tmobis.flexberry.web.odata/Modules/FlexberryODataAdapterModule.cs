namespace Tmobis.Frameworks.Flexberry.Web.OData.Modules;
using NewPlatform.Flexberry.ORM.ODataService;
using Tmobis.Core.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NewPlatform.Flexberry.ORM.ODataService.Extensions;
using NewPlatform.Flexberry.ORM.ODataService.WebApi.Extensions;
using NewPlatform.Flexberry.ORM.ODataService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.OData.Extensions;
using System.Linq;
using Tmobis.Frameworks.Flexberry.Web.OData.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Tmobis.Frameworks.Flexberry.Web.OData.Functions;
using Tmobis.Core.Entities;
using Tmobis.Frameworks.Web.Modules;
using NewPlatform.Flexberry.ORM.ODataServiceCore.Common.Exceptions;
using Tmobis.Frameworks.Flexberry.DataServices;
using Tmobis.Core.DI;
using Tmobis.Frameworks.Flexberry.TownLimit;
using Tmobis.Frameworks.Flexberry.Web.TownLimit;
using NewPlatform.Flexberry.ORM.ODataService.Controllers;
using NewPlatform.Flexberry.ORM.ODataService.Files;
using ICSSoft.STORMNET.Business;
using Tmobis.Frameworks.Flexberry.Web.OData;
using Tmobis.Frameworks.Flexberry.Web.OData.Batch;
using Tmobis.Frameworks.Flexberry.Web.Modules;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Medium.Producers;
using System.Reflection;
using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.Models;

/// <summary>
/// Модуль Flexberry OData API.
/// Содержит OData services, batch handler, DataObject controller, EDM-модель, file route и CRUD events.
/// </summary>
[ModuleRequirement([typeof(FlexberryWebModule)])]
public class FlexberryODataAdapterModule
: ModuleBase, IODataModule, IWebPipelineModule, IWebOptionsModule, IConsumerModule
{
  /// <summary>
  /// Регистрация сервисов в DI .Net.
  /// </summary>
  /// <param name="services">Коллекция сервисов.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  /// <param name="modules">Список модулей.</param>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    RegisterServices(services, configuration, modules);
  }

  /// <inheritdoc />
  public virtual void ConfigureOData(
      ManagementToken manageToken,
      IServiceProvider services)
  {
      // TODO: Fix scoped
      // var employeeFuncs = services.GetService<IEmployeeFunctions>();
      // employeeFuncs?.GetODataFunctions()
      //   .ForEach(manageToken.Functions.Register);
  }

  /// <inheritdoc />
  public void ConfigureWebOptions(MvcOptions options)
  {
    options.Filters.Add<CustomExceptionFilter>();
  }

  /// <inheritdoc />
  public void ConfigureWebPipeline(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    app.UseODataServiceCustom(builder =>
    {
      builder.MapFileRoute();

      var manageToken = ConfigureODataToken(app, configuration, builder, modules);
      ConfigureODataModules(app, manageToken, builder, modules);
      ConfigureODataEvents(app, manageToken);
    });
  }

  protected virtual void RegisterServices(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services.AddOData();
    services.AddSingleton<ODataManagementWrapper>();
    services.AddTransient<IEmployeeFunctions, EmployeeFunctions>();
    services.AddTransient<FlexberryODataBatchHandler>();
    RegisterDataObjectController(services, configuration);
  }

  protected virtual void RegisterDataObjectController(
    IServiceCollection services,
    IConfiguration configuration)
  {
    services.AddOrReplaceTransient((reg) =>
    {
      var fileAccessor = reg.GetService<IDataObjectFileAccessor>();
      var dataService = reg.GetKeyedService<IDataService>("Backend");
      return new DataObjectController(fileAccessor, dataService);
    });
  }

  protected virtual ManagementToken? ConfigureODataToken(
    IApplicationBuilder app,
    IConfiguration configuration,
    IRouteBuilder builder,
    IModuleRegister modules)
  {
    var entityRegister = app.ApplicationServices
      .GetService<IEntityRegister>();
    var assemblies = entityRegister.EntityAssemblies;
    var modelBuilder = new DefaultDataObjectEdmModelBuilder(assemblies, true);
    modelBuilder.PropertyFilter = PropertyFilter;
    var maxPartsPerBatch = int.Parse(configuration["MaxPartsPerBatch"]);
    var managementWrapper = app.ApplicationServices
      .GetService<ODataManagementWrapper>();
    var manageToken = managementWrapper.Token = builder
      .MapFlexberryDataObjectRoute(app.ApplicationServices, modelBuilder,
      messageQuotasMaxPartsPerBatch: maxPartsPerBatch);
    return manageToken;
  }

  protected virtual void ConfigureODataModules(
    IApplicationBuilder app,
    ManagementToken manageToken,
    IRouteBuilder builder,
    IModuleRegister modules)
  {
      var odataModules = modules
        .Where(module => module is IODataModule)
        .Cast<IODataModule>();
      foreach (var module in odataModules)
      {
        module.ConfigureOData(manageToken, app.ApplicationServices);
      }
  }

  protected virtual void ConfigureODataEvents(
    IApplicationBuilder app,
    ManagementToken manageToken)
  {
    ODataEvents.MessageDispatcher = app.ApplicationServices.GetService<IMessageDispatcher>();
    manageToken.Events.CallbackAfterGet = ODataEvents.AfterGet;
    manageToken.Events.CallbackBeforeCreate = ODataEvents.BeforeCreate;
    manageToken.Events.CallbackAfterCreate = ODataEvents.AfterCreate;
    manageToken.Events.CallbackBeforeGet = ODataEvents.BeforeGet;
    manageToken.Events.CallbackBeforeUpdate = ODataEvents.BeforeUpdate;
  }

  private static bool PropertyFilter(PropertyInfo propertyInfo)
  {
    var excludedProperties = new[]
    {
      Information.ExtractPropertyInfo<User>(x => x.Pwd1),
      Information.ExtractPropertyInfo<User>(x => x.Pwd2),
      Information.ExtractPropertyInfo<User>(x => x.PwdEditTime),
    };
    return !excludedProperties.Contains(propertyInfo);
  }
}
