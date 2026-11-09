namespace Tmobis.Backend.EF;
using Tmobis.Core.Medium.Models;

[MessageMetadata("TestGroup", "Test")]
public record TestMessage : DomainEvent
{
    public required string TTOTOTOOT { get; set; }
}