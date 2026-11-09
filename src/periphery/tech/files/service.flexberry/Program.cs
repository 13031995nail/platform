using Tmobis.Auth.ApiKey;
using Tmobis.Core.Modules;
using Tmobis.Files.PrimaryAdapter.Flexberry;
using Tmobis.Frameworks.Flexberry.DataServices.MSSQL;
using Tmobis.Frameworks.Flexberry.Modules;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Frameworks.Web.Modules;

var modules = new ModuleRegister([
    new CoreModule(),
    new FlexberryModule(),
    new WebFrameworkModule(),
    new SwaggerModule(),
    new MSSQLDataServiceModule(),
    new FlexberryFileAdapterModule(),
    new ApiKeyAuthorizationModule()
]);
var appBuilder = new WebAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
