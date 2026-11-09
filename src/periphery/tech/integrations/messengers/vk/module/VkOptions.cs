namespace Tmobis.Integrations.Messengers.Vk;

public sealed class VkOptions
{
    public const string SectionName = "Vk";

    public const string HttpClientName = "Vk";

    public const string MessengerCode = "vk";

    public const string MessengerName = "VK";

    public string ApiUrl { get; set; } = "https://api.vk.com/";

    public string ApiVersion { get; set; } = "5.199";

    public string AccessToken { get; set; } = string.Empty;

    public long GroupId { get; set; }

    public string CallbackSecret { get; set; } = string.Empty;

    public string ConfirmationCode { get; set; } = string.Empty;
}
