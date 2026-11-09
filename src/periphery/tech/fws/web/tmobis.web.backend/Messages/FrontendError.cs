namespace Tmobis.Frameworks.Web.Backend.Messages;

using Tmobis.Core.Medium.Models;

[MessageMetadata("Logging", "FrontendError")]
public record FrontendError : DomainEvent
{
    public required string Message { get; set; }

    public required string Stack { get; set; }

    public required string Component { get; set; }

}
