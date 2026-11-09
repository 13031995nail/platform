using Tmobis.Audit.Json;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Data.EF;
using Tmobis.Data.EF.Postgres;
using Tmobis.Files.Adapter.S3;
using Tmobis.Files.PrimaryAdapter;
using Tmobis.Frameworks.Web.Application;
using Tmobis.Auth.ApiKey;
using Tmobis.Frameworks.Web.Modules;

var modules = new ModuleRegisterBuilder([
  typeof(CoreModule),
  typeof(BusinessCoreModule),
  typeof(WebFrameworkModule),
  typeof(SwaggerModule),
  typeof(JsonAuditModule),
  typeof(EntityFrameworkModule),
  typeof(EFPostgresModule),
  typeof(S3FileAdapterModule),
  typeof(FilesPrimaryAdapterModule),
  typeof(ApiKeyAuthorizationModule)
]);

var builder = new WebAppBuilder(modules, args);
var app = builder.Build();

await app.RunAsync();
