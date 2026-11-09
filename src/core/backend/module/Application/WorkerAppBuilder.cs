namespace Tmobis.Core.Application;
using Microsoft.Extensions.Hosting;
using Tmobis.Core.Modules;

public class WorkerAppBuilder
: AppBuilderBase<HostApplicationBuilder, IHost>
{
  [Obsolete]
  public WorkerAppBuilder(ModuleRegister modules, string[] args)
  : base(modules)
  {
    this.builder = Host.CreateApplicationBuilder(args);
  }

  public WorkerAppBuilder(ModuleRegisterBuilder modules, string[] args)
  : base(modules)
  {
    this.builder = Host.CreateApplicationBuilder(args);
  }

  private readonly HostApplicationBuilder builder;

  protected override HostApplicationBuilder Builder => builder;

  protected override IHost BuildApp() => builder.Build();
}
