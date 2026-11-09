namespace Tmobis.Integrations.Messengers.Max;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class MaxWebhookSubscriptionService(
    IMaxAdapter maxAdapter,
    IOptions<MaxOptions> options,
    ILogger<MaxWebhookSubscriptionService> logger)
    : BackgroundService
{
    private readonly IMaxAdapter maxAdapter = maxAdapter;
    private readonly MaxOptions options = options.Value;
    private readonly ILogger<MaxWebhookSubscriptionService> logger = logger;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (!options.RegisterWebhookOnStart)
        {
            logger.LogInformation(
                "Автоматическая регистрация webhook MAX отключена");
            return;
        }

        if (string.IsNullOrWhiteSpace(options.AccessToken))
        {
            logger.LogWarning(
                "Регистрация webhook MAX пропущена: не задан Max:AccessToken");
            return;
        }

        try
        {
            await maxAdapter.RegisterWebhook(stoppingToken);
            logger.LogInformation(
                "Webhook MAX зарегистрирован по адресу {WebhookUrl}",
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
                "Не удалось зарегистрировать webhook MAX по адресу {WebhookUrl}",
                options.WebhookUrl);
        }
    }
}
