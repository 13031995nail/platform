namespace Tmobis.Core.Medium.Options;

public class MessageListRegisterOptions: MessageOptions
{
    public const string MessageOverride = "MessageListRegisterOverride";
    public Dictionary<string, List<string>> Overrides { get; set; } = new();
}
