namespace Tmobis.Audit;

using Core.Models;

public record AuditRequest
{
    public required IEntity Entity { get; init; }
    
    public required AuditEventType EventType { get; init; }
}
