namespace Tmobis.Business.Core.Models.LikedFolders;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Models;

public class LikedFolder
: BusinessEntity, ILikedFolder
{
    /// <inheritdoc/>
    public virtual Guid UserId { get; set; }

    /// <inheritdoc/>
    public virtual IUser User { get; set; }

    /// <inheritdoc/>
    public virtual string InFavorites { get; set; }
}
