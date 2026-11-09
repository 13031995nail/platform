namespace Tmobis.Business.Common.Personnel;

using Tmobis.Frameworks.Flexberry.Entities;

public interface IBrigade :
  ITeamBase
{
  DetailArray<Qualification, Team> TeamQualifications { get; set; }
}
