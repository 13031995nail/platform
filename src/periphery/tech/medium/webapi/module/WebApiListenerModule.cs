namespace Tmobis.Medium.WebApi;

using Core.Entities;
using Core.Entities.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tmobis.Audit;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;

/// <summary>
/// Модуль первичного адаптера Web API для слоя обмена сообщениями.
/// Содержит фильтрацию, построение выражений поиска и endpoint builders.
/// </summary>
[ModuleRequirement(typeof(WebFrameworkModule), typeof(AuditModule))]
public class WebApiListenerModule : ModuleBase, IWebPipelineModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<IFilterParser, WebApiFilterParser>();
        services.AddTransient<IEntitySearchExpressionBuilder, EntitySearchExpressionBuilder>();
        services.AddTransient<IEntityFilterExpressionBuilder, EntityFilterExpressionBuilder>();
        services.AddTransient<ISortingExpressionBuilder, SortingExpressionBuilder>();
        services.AddKeyedTransient<
            IApiEndpointBuilder, EntityEndpointBuilder>("Entity");
        services.AddKeyedTransient<
            IApiEndpointBuilder, MediumEndpointBuilder>("Medium");
        services.Configure<EntityWebApiOptions>(
            configuration.GetSection(
                EntityWebApiOptions.EntityWebApi));
    }

    /// <inheritdoc/>
    public void ConfigureWebPipeline(
      IApplicationBuilder app,
      IConfiguration configuration,
      IHostEnvironment env,
      IModuleRegister modules)
    {
        ConfigureEndpoints(app);
    }

    protected virtual void ConfigureEndpoints(IApplicationBuilder app)
    {
        var services = app.ApplicationServices;
        var mediumEndpointBuilder = services
            .GetKeyedService<IApiEndpointBuilder>("Medium");
        var entityEndpointBuilder = services
            .GetKeyedService<IApiEndpointBuilder>("Entity");
        var otherEndpointBuilders = services
            .GetServices<IApiEndpointBuilder>();

        app.UseEndpoints(endpoints =>
        {
            mediumEndpointBuilder?.Build(endpoints);
            entityEndpointBuilder?.Build(endpoints);
            foreach (var endpointBuilder in otherEndpointBuilders)
            {
                endpointBuilder.Build(endpoints);
            }
        });
    }
}
