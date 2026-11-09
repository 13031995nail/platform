namespace Tmobis.Frameworks.Web.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль ASP.NET Core web-инфраструктуры.
/// Содержит MVC, controllers, Razor Pages, CORS, health checks и основной HTTP pipeline.
/// </summary>
public class WebFrameworkModule 
: ModuleBase, IWebPipelineModule, IWebOptionsModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    RegisterMvc(services, configuration, modules);
    RegisterControllers(services, configuration, modules);
    RegisterUtils(services, configuration, modules);
    RegisterServices(services, configuration, modules);
  }

  public virtual void ConfigureWebPipeline(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    ConfigureUtils(app, configuration, env, modules);
    ConfigureFiles(app, configuration, env, modules);
    ConfigureRoutes(app, configuration, env, modules);
  }

  public void ConfigureWebOptions(MvcOptions options)
  {
    options.EnableEndpointRouting = false;
  }

  protected virtual void RegisterMvc(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services
      .AddMvcCore((options) => SetWebOptions(options, modules))
      .AddFormatterMappings();
  }

  protected virtual void RegisterControllers(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services.AddControllersWithViews();
    services.AddRazorPages();
    services.AddControllers()
            .AddControllersAsServices();
  }

  protected virtual void RegisterUtils(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services.AddCors();
    services.AddHealthChecks();
  }

  protected virtual void RegisterServices(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services.AddHttpContextAccessor();
  }

  protected virtual void ConfigureFiles(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    app.UseDefaultFiles();
    app.UseStaticFiles();
  }

  protected virtual void ConfigureRoutes(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    app.UseRouting();
    ConfigureAuth(app, configuration, env, modules);
    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapHealthChecks("/health");
    });
  }

  protected virtual void ConfigureAuth(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    app.UseAuthentication();
    app.UseAuthorization();
  }

  protected virtual void ConfigureUtils(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules)
  {
    app.UseCors(builder => builder
      .SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
  }

  protected void SetWebOptions(
    MvcOptions options,
    IModuleRegister modules)
  {
    ArgumentNullException.ThrowIfNull(options);
    ArgumentNullException.ThrowIfNull(modules);

    foreach (var module in modules)
    {
      if (module is IWebOptionsModule webModule)
      {
        webModule.ConfigureWebOptions(options);
      }
    }
  }
}
