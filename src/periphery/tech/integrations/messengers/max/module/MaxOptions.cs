namespace Tmobis.Integrations.Messengers.Max;

public class MaxOptions
{
    public const string Max = "Max";

    public const string HttpClientName = "Max";

    public const string MessengerCode = "max";

    public const string MessengerName = "MAX";

    public string ApiUrl { get; set; } = "https://platform-api2.max.ru/";

    public string AccessToken { get; set; } = string.Empty;

    public string WebhookUrl { get; set; } = string.Empty;

    public string WebhookSecret { get; set; } = string.Empty;

    public bool RegisterWebhookOnStart { get; set; } = true;
}
