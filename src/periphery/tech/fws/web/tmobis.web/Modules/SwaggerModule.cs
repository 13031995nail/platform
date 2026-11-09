namespace Tmobis.Frameworks.Web.Modules;

using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль Swagger/OpenAPI.
/// Содержит API Explorer, генерацию документа <c>v1</c> и Swagger UI.
/// </summary>
[ModuleRequirement(typeof(WebFrameworkModule))]
public sealed class SwaggerModule : ModuleBase, IWebPipelineModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            foreach (var module in modules)
            {
                if (module is not ISwaggerOptionsModule optionsModule)
                    continue;
                
                optionsModule.InitializeSwagger(c);
            }
            
            c.SwaggerDoc(
                "v1",
                new() { Title = "Tmobis.Api", Version = "v1" });
        });
    }

    public void ConfigureWebPipeline(
        IApplicationBuilder app,
        IConfiguration configuration,
        IHostEnvironment env,
        IModuleRegister modules)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tmobis.Api.v1"); });
    }
}
