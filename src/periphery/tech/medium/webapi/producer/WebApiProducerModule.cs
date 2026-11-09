namespace Tmobis.Medium.WebApi.Producer;
using Tmobis.Core.Modules;
using Tmobis.Core.Medium.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Модуль вторичного адаптера Web API для слоя обмена сообщениями.
/// </summary>
public class WebApiProducerModule : ModuleBase
{
    public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<WebApiProducer>();
        services.Configure<WebApiProducerOptions>(
            configuration.GetSection(WebApiProducerOptions.WebApiProducer));
    }
}
