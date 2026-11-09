namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

/// <summary>
/// Brigade.
/// </summary>
[AutoAltered]
[NotStored(false)]
[ClassStorage("Brigade")]
[AccessType(AccessType.none)]
[View("BrigadeE", ["Leader"])]
[AssociatedDetailView("BrigadeE", "TeamQualifications", "TeamQualificationD", true, "", "", true, new string[] { "" })]
public class Brigade :
  Team,
  IBrigade
{

  public virtual DetailArray<Qualification, Team> TeamQualifications { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {
    /// <summary>
    /// "BrigadeE" view.
    /// </summary>
    public static View BrigadeE
      => Information.GetView("BrigadeE", typeof(Brigade));
  }
}
