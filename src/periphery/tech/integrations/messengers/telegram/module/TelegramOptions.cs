namespace Tmobis.Integrations.Messengers.Telegram;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    public const string HttpClientName = "Telegram";

    public const string MessengerCode = "telegram";

    public const string MessengerName = "Telegram";

    public string BotToken { get; set; } = string.Empty;

    public Uri? WebhookUrl { get; set; }

    public string WebhookSecret { get; set; } = string.Empty;

    public bool RegisterWebhookOnStart { get; set; }
}
