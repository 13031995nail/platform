namespace Tmobis.Medium.Signalr;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Medium.Signalr.ModuleOptions;

/// <summary>
/// Модуль SignalR для слоя обмена сообщениями.
/// Содержит hub, producer и реестр активных соединений.
/// </summary>
public class SignalrModule : ModuleBase, IWebPipelineModule, IProducerModule
{
    /// <inheritdoc/>
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddSignalR();
        services.AddTransient<SignalRProducer>();
        services.AddSingleton<SignalRConnections>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, SignalRJwtBearerEvents>());
    }

  public virtual void ConfigureWebPipeline(
      IApplicationBuilder app,
      IConfiguration configuration,
      IHostEnvironment env,
      IModuleRegister modules)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<SignalRAdapterHub>("/signalr"); 
        });
    }
}
