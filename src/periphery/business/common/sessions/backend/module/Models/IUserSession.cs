namespace Tmobis.Business.Common.Sessions.Models;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

[InternalEntity]
public interface IUserSession : IActualEntity, IRegisteredEntity
{
    public DateTime StartedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public Guid UserId { get; set; }
    
    public IUser User { get; set; }

    public string? DeviceInfo { get; set; }

    public string Login { get; set; }

    public string Ip { get; set; }
}
