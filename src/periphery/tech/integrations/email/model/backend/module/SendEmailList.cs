namespace Tmobis.Integrations.Email;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "email")]
public record SendEmailList : DomainCommand<SendEmailResult>
{
    public required List<SendEmail> EmailRequests { get; init; }
}
