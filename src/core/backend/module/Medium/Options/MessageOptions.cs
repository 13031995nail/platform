namespace Tmobis.Core.Medium.Options;

public class MessageOptions
{
    public const string MessageOverride = "MessageOverride";
    public Dictionary<string, List<string>> Overrides { get; set; }
}
