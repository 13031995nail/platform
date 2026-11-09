namespace Tmobis.Core.Medium.Options;

public class MessageProducerOptions : MessageOptions
{
    public const string MessageOverride = "MessageProducerOverride";
    public Dictionary<string, string> Overrides { get; set; } = new();
}
