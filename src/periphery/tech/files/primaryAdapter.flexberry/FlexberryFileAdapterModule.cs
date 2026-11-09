namespace Tmobis.Files.PrimaryAdapter.Flexberry;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewPlatform.Flexberry.ORM.ODataService.Files;
using NewPlatform.Flexberry.ORM.ODataService.WebApi.Controllers;
using System;
using System.Configuration;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Flexberry модуль HTTP-доступа к файлам.
/// Содержит file controller и маршрут <c>api/fileload</c>.
/// </summary>
[ModuleRequirement([typeof(WebFrameworkModule)])]
public class FlexberryFileAdapterModule : ModuleBase, IWebPipelineModule
{
  private readonly string fileControllerPath = "api/fileload";

  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    RegisterDataObjectFileAccessor(services, configuration);
  }

  /// <summary>
  /// Регистрация реализации IDataObjectFileAccessor.
  /// </summary>
  protected virtual void RegisterDataObjectFileAccessor(
    IServiceCollection services,
    IConfiguration configuration)
  {
    string baseUriRaw = configuration["BackendRoot"];
    if (string.IsNullOrEmpty(baseUriRaw))
    {
      throw new ConfigurationErrorsException(
          "BackendRoot is not specified "
          + "in Configuration or environment variables.");
    }

    var baseUri = new Uri(baseUriRaw);
    string uploadPath = configuration["UploadUrl"];
    services.AddSingleton<
      IDataObjectFileAccessor>((provider) =>
        new DefaultDataObjectFileAccessor(
          baseUri,
          fileControllerPath,
          uploadPath,
          null));

    services.AddTransient<FileController>();
  }

  public void ConfigureWebPipeline(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    var webApp = app as IEndpointRouteBuilder;
    webApp.MapControllerRoute("file", fileControllerPath, new
    {
      controller = "File",
      action = "FileAction"
    });
  }
}
