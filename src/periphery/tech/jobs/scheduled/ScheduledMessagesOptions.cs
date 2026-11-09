namespace Tmobis.Jobs.Scheduled;

public class ScheduledMessagesOptions
{
    public const string ScheduledMessages = "ScheduledMessages";

    public Dictionary<string, string> Schedule { get; set; } = [];
}