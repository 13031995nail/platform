namespace Tmobis.Audit.Json;

using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Models;

public class AuditEvent
: BusinessEntity, IAuditEvent
{
    public virtual required AuditEventType EventType { get; set; }
    
    public virtual required Guid EntityId { get; set; }
    
    public virtual required string EntityType { get; set; }
    
    [SerializedJson]
    public virtual string? EntitySnapshot { get; set; }
    
    public virtual Guid? UserId { get; set; }
    
    public virtual IUser? User { get; set; }
    
    public virtual required string? Source { get; set; }
}
