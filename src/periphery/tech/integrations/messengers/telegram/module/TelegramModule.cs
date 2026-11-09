namespace Tmobis.Integrations.Messengers.Telegram;

using global::Telegram.Bot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;
using Tmobis.Integrations.Messengers.Telegram.Authorization;
using Tmobis.Integrations.Messengers.Telegram.Messages;
using Tmobis.Medium.WebApi;

/// <summary>
/// Модуль интеграции с Telegram.
/// Содержит bot client, webhook-авторизацию, adapter и service управления webhook.
/// </summary>
[ModuleRequirement(
    typeof(MessengerModule),
    typeof(WebFrameworkModule),
    typeof(WebApiListenerModule))]
public sealed class TelegramModule
    : ModuleBase, IMessageModule, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<TelegramOptions>(
            configuration.GetSection(TelegramOptions.SectionName));
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                TelegramAuthorizationPolicies.Webhook,
                policy => policy.AddRequirements(
                    new TelegramWebhookAuthorizationRequirement()));
        });
        services.AddSingleton<
            IAuthorizationHandler,
            TelegramWebhookAuthorizationHandler>();
        services.AddListenedMessage<TelegramUpdateReceived>("WebApi");
        services.AddHttpClient(TelegramOptions.HttpClientName)
            .RemoveAllLoggers();
        services.AddTransient<ITelegramBotClient>(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<TelegramOptions>>()
                .Value;
            if (string.IsNullOrWhiteSpace(options.BotToken))
            {
                throw new InvalidOperationException(
                    "Не задан Telegram:BotToken");
            }

            var httpClient = provider
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(TelegramOptions.HttpClientName);
            return new TelegramBotClient(options.BotToken, httpClient);
        });
        services.AddTransient<ITelegramAdapter, TelegramAdapter>();
        services.AddHostedService<TelegramWebhookSubscriptionService>();
    }
}
