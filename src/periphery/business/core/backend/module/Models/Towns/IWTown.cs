namespace Tmobis.Business.Core.Models.Towns;

using Tmobis.Business.Core.TownLimit;
using Tmobis.Core.Models;

[TownLimited("Town")]
public interface IWTown : IEntity
{
    Guid TownId { get; set; }

    ITown Town { get; set; }
}
