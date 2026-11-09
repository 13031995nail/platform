namespace Tmobis.Audit.Json;

using Core.Entities;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Models;

[InternalEntity(EntityVisibility.Read)]
public interface IAuditEvent: IRegisteredEntity
{
    public AuditEventType EventType { get; set; }

    public Guid EntityId { get; set; }
    
    public string EntityType { get; set; }

    [SerializedJson]
    public string? EntitySnapshot { get; set; }

    public Guid? UserId { get; set; }

    public IUser? User { get; set; }
    
    public string? Source { get; set; }
}
