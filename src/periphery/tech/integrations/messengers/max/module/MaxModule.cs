namespace Tmobis.Integrations.Messengers.Max;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Integrations.Messengers.Max.Authorization;
using Tmobis.Integrations.Messengers.Max.Messages;
using Tmobis.Medium.WebApi;

/// <summary>
/// Модуль интеграции с MAX.
/// Содержит API client, webhook-авторизацию, adapter и service подписки webhook.
/// </summary>
[ModuleRequirement(
    typeof(MessengerModule),
    typeof(WebApiListenerModule))]
public class MaxModule
    : ModuleBase, IMessageModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<MaxOptions>(
            configuration.GetSection(MaxOptions.Max));
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                MaxAuthorizationPolicies.Webhook,
                policy => policy.AddRequirements(
                    new MaxWebhookAuthorizationRequirement()));
        });
        services.AddSingleton<
            IAuthorizationHandler,
            MaxWebhookAuthorizationHandler>();
        services.AddListenedMessage<MaxMessageCreated>("WebApi");
        services.AddHttpClient(
            MaxOptions.HttpClientName,
            (provider, client) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<MaxOptions>>()
                    .Value;
                client.BaseAddress = new Uri(options.ApiUrl);
                if (!string.IsNullOrWhiteSpace(options.AccessToken))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "Authorization",
                        options.AccessToken.Trim());
                }
            });

        services.AddTransient<IMaxAdapter, MaxAdapter>();
        services.AddHostedService<MaxWebhookSubscriptionService>();
    }
}
