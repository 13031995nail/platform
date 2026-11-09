namespace Tmobis.Business.Core.Models.Users;

using Tmobis.Business.Core.Models.Towns;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public class User
: ActualEntity, IUser
{
    /// <inheritdoc/>
    public string Login { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string? Email { get; set; }

    /// <inheritdoc/>
    public virtual string? UserId { get; set; }

    /// <inheritdoc/>
    public virtual Guid TownId { get; set; }

    /// <inheritdoc/>
    public virtual ITown Town { get; set; }

    /// <inheritdoc/>
    public virtual DateTime? PwdEditTime { get; set; }

    /// <inheritdoc/>
    public virtual string? Pwd1 { get; set; }

    /// <inheritdoc/>
    public virtual string? Pwd2 { get; set; }

    /// <inheritdoc/>
    public virtual bool? IsComplexPwd { get; set; }

    /// <inheritdoc/>
    public virtual string? UserPwd { get; set; }
}
