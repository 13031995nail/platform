namespace Tmobis.Frameworks.Web.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Tmobis.Core.Modules;

public interface IWebPipelineModule : IModule
{
  void ConfigureWebPipeline(
    IApplicationBuilder app,
    IConfiguration configuration,
    IHostEnvironment env,
    IModuleRegister modules);
}
