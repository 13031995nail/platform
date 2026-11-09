#pragma warning disable SA1200 // Using directives should be placed correctly
using Tmobis.Reports.Controller;
using Tmobis.Reports.Stimulsoft.Client.Adapter;
using Tmobis.Jobs.Quartz;
using Tmobis.Business.Common.Sessions.Modules;
using Tmobis.Audit.Json;
using Tmobis.Logging.Syslog;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Data.EF;
using Tmobis.Data.EF.Postgres;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Backend.Modules;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Jobs.Scheduled;
using Tmobis.Medium.WebApi;
using Tmobis.Objects.EF;
using Tmobis.Business.Process.WorkShift;
using Tmobis.Auth.JwtAuth.Modules;
using Tmobis.Auth.Keycloak.Modules;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.Signalr;
using Tmobis.Auth.UserSessions;
using Tmobis.Notification;
#pragma warning restore SA1200 // Using directives should be placed correctly

var modules = new ModuleRegisterBuilder([
    typeof(BusinessCoreModule),
    typeof(WebFrameworkModule),
    typeof(WebBackendModule),
    typeof(JwtAuthModule),
    typeof(MessageScheduleModule),
    typeof(SwaggerModule),
    typeof(WebApiListenerModule),
    typeof(UserSessionsModule),
    typeof(WorkShiftModule),
    typeof(NotificationModule),
    typeof(EntityFrameworkModule),
    typeof(EFPostgresModule),
    typeof(JsonAuditModule),
    typeof(KeycloakAuthModule),
    typeof(QuartzAdapterModule),
    typeof(TestEFObjectsModule),
    typeof(RabbitListenerModule),
    typeof(SignalrModule),
    typeof(UserSessionAuthorizationModule),
    typeof(SyslogModule),
    typeof(ReportControllerModule),
    typeof(StimulsoftAdapterModule)
]);

var builder = new WebAppBuilder(modules, args);
var app = builder.Build();

await app.RunAsync();
