namespace Tmobis.Business.Common.Sessions.Messages;
using Tmobis.Core.Medium.Models;

[MessageMetadata("UserSessions", "StopSession")]
public record UserSessionStopSession : DomainEvent
{
    required public Guid? SessionId { get; init; }
}