namespace Tmobis.Integrations.Email;
using Tmobis.Core.Medium.Models;
public record SendEmailFile : Message<SendEmailResult>
{
    public string? FileName { get; init; }
    public Guid FileId { get; init; }
}
