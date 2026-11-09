namespace Tmobis.Reports.Adapter;
using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Reports.Adapter.UseCases;

/// <summary>
/// Модуль бизнес-обработки подсистемы отчетов и печатных форм.
/// </summary>
[ModuleRequirement([typeof(ReportModelModule)])]
public sealed class BaseReportAdapterModule : ModuleBase
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IReportUseCase, ReportUseCase>();
    }
}