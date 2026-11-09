namespace Tmobis.Business.Core.Models.LikedFolders;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Models;

public interface ILikedFolder : IRegisteredEntity, IWUser
{
    public string InFavorites { get; set; }
}
