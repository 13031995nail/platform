namespace Tmobis.Integrations.Messengers.Mattermost;

using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль интеграции с Mattermost.
/// Содержит HTTP client, adapter и WebSocket service для приёма событий.
/// </summary>
[ModuleRequirement(typeof(MessengerModule))]
public sealed class MattermostModule
    : ModuleBase, IMessageModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<MattermostOptions>(
            configuration.GetSection(MattermostOptions.SectionName));
        services.AddHttpClient(
            MattermostOptions.HttpClientName,
            (provider, client) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<MattermostOptions>>()
                    .Value;
                if (!string.IsNullOrWhiteSpace(options.ServerUrl))
                {
                    client.BaseAddress = new Uri(
                        options.ServerUrl.TrimEnd('/') + "/");
                }

                if (!string.IsNullOrWhiteSpace(options.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue(
                            "Bearer",
                            options.AccessToken.Trim());
                }
            });
        services.AddTransient<IMattermostAdapter, MattermostAdapter>();
        services.AddHostedService<MattermostWebSocketService>();
    }
}
