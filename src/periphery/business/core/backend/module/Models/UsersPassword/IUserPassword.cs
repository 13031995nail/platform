namespace Tmobis.Business.Core.Models.UsersPassword;

using Tmobis.Core.Entities;
using Tmobis.Core.Models;

[InternalEntity]
public interface IUserPassword
: IRegisteredEntity
{
    string PwdHash { get; set; }
    
    Guid UserId { get; set; }
}
