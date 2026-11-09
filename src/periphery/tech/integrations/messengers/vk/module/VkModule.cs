namespace Tmobis.Integrations.Messengers.Vk;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Medium.WebApi;

/// <summary>
/// Модуль интеграции с VK.
/// Содержит API client, adapter и HTTP endpoint для приёма событий.
/// </summary>
[ModuleRequirement(
    typeof(MessengerModule),
    typeof(WebApiListenerModule))]
public sealed class VkModule
    : ModuleBase, IMessageModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<VkOptions>(
            configuration.GetSection(VkOptions.SectionName));
        services.AddHttpClient(
            VkOptions.HttpClientName,
            (provider, client) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<VkOptions>>()
                    .Value;
                client.BaseAddress = new Uri(options.ApiUrl);
            });
        services.AddTransient<IVkAdapter, VkAdapter>();
        services.AddTransient<IApiEndpointBuilder, VkEndpointBuilder>();
    }
}
