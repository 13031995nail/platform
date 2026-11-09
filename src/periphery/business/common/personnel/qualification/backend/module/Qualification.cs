namespace Tmobis.Business.Common.Personnel;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

[AutoAltered]
[AccessType(AccessType.@this)]
[View("AuditView", new string[] { "Creator", "CreateTime", "Editor", "EditTime"})]
[View("QualificationE", new string[] { "Name", "QualificationGroupName", "Relevance"})]
[View("QualificationL", new string[] { "Name", "QualificationGroupName", "Relevance"})]
public class Qualification : FlexberryDictionary, IQualification
{
  [StrLen(255)]
  [NotNull]
  public virtual string QualificationGroupName { get; set; }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {

    /// <summary>
    /// "AuditView" view.
    /// </summary>
    public static View AuditView
     => Information.GetView("AuditView", typeof(Qualification));

    /// <summary>
    /// "QualificationE" view.
    /// </summary>
    public static View QualificationE
      => Information.GetView("QualificationE", typeof(Qualification));

    /// <summary>
    /// "QualificationL" view.
    /// </summary>
    public static View QualificationL
      => Information.GetView("QualificationL", typeof(Qualification));
  }
}
