using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Data.EF;
using Tmobis.Data.EF.Postgres;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Backend.Modules;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Jobs.Quartz;
using Tmobis.Jobs.Scheduled;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.WebApi;
using Tmobis.Notification;
using Tmobis.Notification.Send.Rabbit.Modules;

var modules = new ModuleRegisterBuilder([
    typeof(BusinessCoreModule),
    typeof(WebFrameworkModule),
    typeof(WebBackendModule),
    typeof(MessageScheduleModule),
    typeof(QuartzAdapterModule),
    typeof(NotificationServiceModule),
    typeof(RabbitProducerModule),
    typeof(SwaggerModule),
    typeof(WebApiListenerModule),
    typeof(EntityFrameworkModule),
    typeof(EFPostgresModule),
    typeof(NotificationModule)
]);
var appBuilder = new WebAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
