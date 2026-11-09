namespace Tmobis.Business.Core.Models.UsersPassword;
using Tmobis.Core.Models;

public class UserPassword
: BusinessEntity, IUserPassword
{
    public string PwdHash { get; set; }
    public virtual Guid UserId { get; set; }
}
