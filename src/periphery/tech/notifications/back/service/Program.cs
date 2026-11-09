using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Data.EF;
using Tmobis.Data.EF.Postgres;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Backend.Modules;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Medium.RabbitMQ;
using Tmobis.Medium.WebApi;
using Tmobis.Notification;
using Tmobis.Notification.Mattermost;
using Tmobis.Notification.Max;
using Tmobis.Notification.Sender;
using Tmobis.Notification.Telegram;
using Tmobis.Notification.Vk;
using Tmobis.Notification.WebSocket;

var modules = new ModuleRegisterBuilder([
    typeof(BusinessCoreModule),
    typeof(WebFrameworkModule),
    typeof(WebBackendModule),
    typeof(RabbitProducerModule),
    typeof(SwaggerModule),
    typeof(WebApiListenerModule),
    typeof(EntityFrameworkModule),
    typeof(EFPostgresModule),
    typeof(NotificationSenderModule),
    typeof(NotificationModule),
    typeof(RabbitListenerModule),
    typeof(NotificationWebSocketModule),
    typeof(NotificationMaxModule),
    typeof(NotificationTelegramModule),
    typeof(NotificationVkModule),
    typeof(NotificationMattermostModule),
]);
var appBuilder = new WebAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
