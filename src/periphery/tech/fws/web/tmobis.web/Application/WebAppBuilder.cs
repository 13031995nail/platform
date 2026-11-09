namespace Tmobis.Frameworks.Web.Application;
using Tmobis.Core.Application;
using Microsoft.AspNetCore.Builder;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Core.Modules;
using Microsoft.Extensions.Hosting;

public class WebAppBuilder
: AppBuilderBase<WebApplicationBuilder, WebApplication>
{
  [Obsolete]
  public WebAppBuilder(ModuleRegister modules, string[] args)
  : base(modules)
  {
    this.args = args;
    this.builder = WebApplication.CreateBuilder(args);
  }

  public WebAppBuilder(ModuleRegisterBuilder modules, string[] args)
  : base(modules)
  {
    this.args = args;
    this.builder = WebApplication.CreateBuilder(args);
  }

  private readonly string[] args;
  private readonly WebApplicationBuilder builder;

  protected override WebApplicationBuilder Builder => builder;

  protected override WebApplication BuildApp() => builder.Build();

  protected override void ConfigureModules()
  {
    if (App == null)
    {
      throw new InvalidOperationException("Приложение не было построено.");
    }

    base.ConfigureModules();

    foreach (var module in Modules)
    {
      if (module is IWebPipelineModule webModule)
      {
        webModule.ConfigureWebPipeline(
          App, Builder.Configuration, App.Environment, Modules);
      }
    }
  }
}
