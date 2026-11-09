namespace Tmobis.Reports.Stimulsoft.Client.Adapter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Core.Modules;
using Tmobis.Reports.Adapter;
using Tmobis.Reports.Adapter.SecondaryAdapters;
using Tmobis.Reports.Stimulsoft.Client.Adapter.Adapters;
using Tmobis.Reports.Stimulsoft.Client.Adapter.Options;

/// <summary>
/// Модуль адаптера Stimulsoft для формирования отчетов и печатных форм.
/// </summary>
[ModuleRequirement([typeof(BaseReportAdapterModule)])]
public sealed class StimulsoftAdapterModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IReportServiceAdapter, StimulsoftAdapter>();
    services.AddHttpClient<StimulsoftAdapter>();
    services.AddHttpClient<StimulsoftAdapter>((provider, client) =>
    {
      var options = provider.GetService<
          IOptions<StimulsoftAdapterOptions>>()?.Value
          ?? throw new InvalidOperationException(
              "Опции не зарегистрированы");

      client.Timeout = options.Timeout;
    });
    services.Configure<StimulsoftAdapterOptions>(
      configuration.GetSection(StimulsoftAdapterOptions.Stimulsoft));
  }
}