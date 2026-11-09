namespace Tmobis.Business.Core.Models.Users;

using Tmobis.Core.Models;

public interface IWUser : IEntity
{
    Guid UserId { get; set; }

    IUser User { get; set; }
}
