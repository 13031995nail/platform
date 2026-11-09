using Microsoft.Extensions.Hosting;
using Tmobis.Core.Application;
using Tmobis.Core.Modules;
using Tmobis.Files.Adapter.S3;
using Tmobis.Files.Adapter.S3.Sample;

var modules = new ModuleRegisterBuilder([
  typeof(S3FileAdapterModule),
  typeof(S3SampleModule)
]);

var appBuilder = new WorkerAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();
