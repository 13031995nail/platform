using Microsoft.Extensions.Hosting;
using Tmobis.Business.Core.Flexberry.Modules;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Application;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Flexberry.Converters.OldFile.Modules;
using Tmobis.Frameworks.Flexberry.DataServices.MSSQL;
using Tmobis.Frameworks.Flexberry.Modules;

var modules = new ModuleRegister([
    new CoreModule(),
    new FlexberryModule(),
    new BusinessCoreModule(),
    new MSSQLDataServiceModule(),
    new FlexberryBusinessCoreModule(),
    new FlexberryOldFileConverterModule()
]);
var appBuilder = new WorkerAppBuilder(modules, args);
var app = appBuilder.Build();
await app.RunAsync();