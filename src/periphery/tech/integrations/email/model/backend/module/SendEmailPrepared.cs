namespace Tmobis.Integrations.Email;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "email")]
public record SendEmailPrepared : RemoteCommand<SendEmailResult>
{
    public required List<string> Addresses { get; init; }

    public required string Subject { get; init; }

    public required string Body { get; init; }

    public required bool HighPriority { get; init; } = false;

    public Dictionary<string, byte[]>? Files { get; init; } = null;

    public required bool DiffLetters { get; init; } = false;

}
