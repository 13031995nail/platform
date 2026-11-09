namespace Tmobis.Integrations.Messengers.Mattermost;

public sealed class MattermostOptions
{
    public const string SectionName = "Mattermost";

    public const string HttpClientName = "Mattermost";

    public const string MessengerCode = "mattermost";

    public const string MessengerName = "Mattermost";

    public string ServerUrl { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string BotUserId { get; set; } = string.Empty;

    public bool ListenForMessages { get; set; } = true;

    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);
}
