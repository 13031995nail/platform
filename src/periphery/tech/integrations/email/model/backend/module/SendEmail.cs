namespace Tmobis.Integrations.Email;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "email")]
public record SendEmail : DomainCommand<SendEmailResult>
{
    public required List<string> Addresses { get; init; }

    public required string Subject { get; init; }

    public required string Body { get; init; }

    public required bool HighPriority { get; init; } = false;

    public List<SendEmailFile>? FileIds { get; init; } = null;

    public required bool DiffLetters { get; init; } = false;
}
