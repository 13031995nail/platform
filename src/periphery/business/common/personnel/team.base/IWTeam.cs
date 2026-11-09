namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;

public interface IWTeam
{
  [PropertyStorage(["Team"])]
  public ITeamBase? Team { get; set; }
}
