namespace Tmobis.Reports.Controller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Reports.Adapter;

/// <summary>
/// Модуль первичного адаптера подсистемы отчетов и печатных форм. 
/// </summary>
[ModuleRequirement([typeof(BaseReportAdapterModule)])]
public sealed class ReportControllerModule : ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<ReportController>();
  }
}
