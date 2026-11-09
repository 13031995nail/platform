#pragma warning disable SA1200 // Using directives should be placed correctly
using Tmobis.Backend.Modules;
using Tmobis.Backend.Objects;
using Tmobis.Business.Common.Sessions.Flexberry.Modules;
using Tmobis.Business.Common.Sessions.Modules;
using Tmobis.Business.Core.Flexberry.Modules;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Files.PrimaryAdapter.Flexberry;
using Tmobis.Flexberry.Autoincrement.Modules;
using Tmobis.Frameworks.Flexberry.DataServices.Postgres;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Frameworks.Flexberry.Web.Modules;
using Tmobis.Frameworks.Flexberry.Web.OData.Modules;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Auth.JwtAuth.Modules;
using Tmobis.Auth.Password.Modules;
using Tmobis.Frameworks.Web.Backend.Modules;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Jobs.Scheduled;
using Tmobis.Medium.WebApi;

#pragma warning restore SA1200 // Using directives should be placed correctly

var modules = new ModuleRegisterBuilder([
    typeof(BusinessCoreModule),
    typeof(FlexberryModule),
    typeof(PostgresDataServiceModule),
    typeof(WebFrameworkModule),
    typeof(WebBackendModule),
    typeof(FlexberryBusinessCoreModule),
    typeof(FlexberryWebModule),
    typeof(FlexberryODataAdapterModule),
    typeof(JwtAuthModule),
    typeof(BasicPasswordAuthModule),
    typeof(AutoincrementModule),
    typeof(MessageScheduleModule),
    typeof(SwaggerModule),
    typeof(WebApiListenerModule),
    typeof(UserSessionsModule),
    typeof(ObjectsModule),
    typeof(FlexberrySessionModule),
    typeof(FlexberryFileAdapterModule),
    typeof(BackendModule)
]);

var builder = new WebAppBuilder(modules, args);
var app = builder.Build();
await app.RunAsync();