namespace Tmobis.Business.Common.Sessions.Models;

using Tmobis.Core.Models;
using Tmobis.Business.Core.Models.Users;

public class UserSession
    : ActualEntity, IUserSession
{
    public virtual DateTime StartedAt { get; set; }

    public virtual DateTime? ClosedAt { get; set; }

    public virtual required IUser User { get; set; }
    
    public virtual Guid UserId {get; set;}

    public virtual string? DeviceInfo { get; set; }

    public virtual string Login { get; set; } = string.Empty;

    public virtual string Ip { get; set; } = string.Empty;
}
