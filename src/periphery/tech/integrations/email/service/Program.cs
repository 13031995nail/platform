using Tmobis.Auth.ApiKey;
using Tmobis.Core.Modules;
using Tmobis.Data.EF;
using Tmobis.Data.EF.Postgres;
using Tmobis.Files.Provider.Remote;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Integrations.Email.Consumer;
using Tmobis.Integrations.Email.Sender.Consumer;
using Tmobis.Integrations.Email.Sender.MailKit;
using Tmobis.Integrations.Email.Service.Modules;
using Tmobis.Medium.WebApi;

var modules = new ModuleRegisterBuilder([
    typeof(CoreModule),
    typeof(WebFrameworkModule),
    typeof(ApiKeyAuthorizationModule),
    typeof(SwaggerModule),
    typeof(WebApiListenerModule),
    typeof(RemoteFileProviderModule),
    typeof(MailKitSenderModule),
    typeof(EmailConsumerModule),
    typeof(EmailServiceModule),
    typeof(EmailSenderConsumerModule),
    typeof(EntityFrameworkModule),
    typeof(EFPostgresModule)
]);
var appBuilder = new WebAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
