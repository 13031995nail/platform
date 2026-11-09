namespace Tmobis.Reports.Stimulsoft.Service.Modules;

using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Reports.Stimulsoft.Service.Options;
using Tmobis.Reports.Stimulsoft.Service.UseCases;
using Microsoft.Extensions.Options;
using System;
using Tmobis.Reports.Models;
using Tmobis.Core.Medium.Listeners;
using Tmobis.Core.Medium.Modules;
using Tmobis.Reports.Stimulsoft.Service.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Medium.RabbitMQ.SecondaryAdapters;
using Tmobis.Core.DI;

/// <summary>
/// Модуль сервиса отчетов и печатных форм.
/// </summary>
public sealed class ReportServiceModule : ModuleBase, IConsumerModule
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IFileReportLoader, FileReportLoader>();
    services.AddTransient<ISqlReportBuilder, SqlReportBuilder>();
    services.AddTransient<IFileReportBuilder, FileReportBuilder>();
    services.AddTransient<IReportExporter, ReportExporter>();
    services.AddTransient<IMassReportExporter, MassReportExporter>();

    services.Configure<ReportServiceOptions>(
      configuration.GetSection(ReportServiceOptions.ReportService));
    services.AddListenedMessage<GetMassReportRequest>("RabbitMQ");
    services.AddListenedMessage<GetBigReportRequest>("RabbitMQ");
    services.AddProducerMessage<GetMassReportResponse>("RabbitProducer");
  }

  /// <inheritdoc/>
  public override void Configure(
    IServiceProvider services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Configure(services, configuration, modules);
    var options = services.GetService<IOptions<ReportServiceOptions>>()?.Value;
    ConfigureLicense(options);
    ConfigureFonts(configuration, options);
  }

  private void ConfigureLicense(ReportServiceOptions? options)
  {
    global::Stimulsoft.Base.StiLicense.Key = options?.StimulsoftLicenseKey;
  }

  private void ConfigureFonts(
    IConfiguration configuration,
    ReportServiceOptions options)
  {
    var contentPath = configuration?
      .GetValue<string>(WebHostDefaults.ContentRootKey);
    foreach (var fontFile in Directory
      .GetFiles(Path.Combine(contentPath, options.FontsPath), "*.ttf"))
    {
      global::Stimulsoft.Base.StiFontCollection.AddFontFile(fontFile);
    }
  }
}
