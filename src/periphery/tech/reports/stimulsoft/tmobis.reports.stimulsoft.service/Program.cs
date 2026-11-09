using Tmobis.Auth.ApiKey;
using Tmobis.Core.Modules;
using Tmobis.Files.Provider.Remote;
using Tmobis.Files.Uploader.Local.Modules;
using Tmobis.Frameworks.Flexberry.DataServices.Combined;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Logging.Log4Net;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.WebApi;
using Tmobis.Reports;
using Tmobis.Reports.Adapter;
using Tmobis.Reports.Flexberry;
using Tmobis.Reports.Stimulsoft.Client.Adapter;
using Tmobis.Reports.Stimulsoft.Service.Modules;

var modules = new ModuleRegister([
    new CoreModule(),
    new FlexberryModule(),
    new CombinedDataServiceModule(),
    new WebFrameworkModule(),
    new ApiKeyAuthorizationModule(),
    new ReportServiceModule(),
    new ReportModelModule(),
    new SwaggerModule(),
    new FlexberryReportModelModule(),
    new BaseReportAdapterModule(),
    new StimulsoftAdapterModule(),
    new LocalFileUploaderModule(),
    new WebApiListenerModule(),
    new RemoteFileProviderModule(),
    new RabbitListenerModule(),
    new RabbitProducerModule(),
    new Log4NetServiceModule()
]);
var appBuilder = new WebAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
