namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Frameworks.Flexberry.Entities;

public interface ITeam :
  ITeamBase
{
  // [NotNull]
  [PropertyStorage("Leader")]
  TeamEmployee Leader { get; set; }

  DetailArray<Employee, Team> TeamEmployees { get; set; }
}
