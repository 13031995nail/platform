namespace Tmobis.Business.Common.Sessions.Messages;
using Tmobis.Core.Medium.Models;

[MessageMetadata("UserSessions", "SetDeviceData")]
public record UserSessionSetDeviceData : DomainEvent
{
    required public string DeviceInfo { get; init; }
}