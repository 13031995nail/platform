namespace Tmobis.Integrations.Messengers.Telegram;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class TelegramWebhookSubscriptionService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<TelegramOptions> optionsAccessor,
    ILogger<TelegramWebhookSubscriptionService> logger)
    : BackgroundService
{
    private readonly TelegramOptions options = optionsAccessor.Value;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await Task.Yield();

        if (!options.RegisterWebhookOnStart)
        {
            logger.LogInformation(
                "Автоматическая регистрация webhook Telegram отключена");
            return;
        }

        if (string.IsNullOrWhiteSpace(options.BotToken)
            || options.WebhookUrl == null
            || string.IsNullOrWhiteSpace(options.WebhookSecret))
        {
            logger.LogWarning(
                "Регистрация webhook Telegram пропущена: настройки Telegram заполнены не полностью");
            return;
        }

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var telegramAdapter = scope.ServiceProvider
                .GetRequiredService<ITelegramAdapter>();
            await telegramAdapter.RegisterWebhook(stoppingToken);
            logger.LogInformation(
                "Webhook Telegram зарегистрирован по адресу {WebhookUrl}",
                options.WebhookUrl);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Не удалось зарегистрировать webhook Telegram по адресу {WebhookUrl}",
                options.WebhookUrl);
        }
    }
}
