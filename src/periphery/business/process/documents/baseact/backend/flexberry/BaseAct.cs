namespace Tmobis.Business.Process.Documents.Flexberry;

using System.Runtime.Serialization;
using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Process.Documents.Base.Flexberry;

/// <summary>
/// Документ.
/// </summary>
[ClassStorage("BaseAct")]
[AutoAltered]
[Caption("Документ")]
[AccessType(AccessType.none)]
[View("BaseActE", [
  "RegDate", "ActStatus", "ActNumber", "ChangeStatusDate", "OrganizationName",
  "OrganizationPhone", "OrganizationAddress", "ActNumberIndex",
  "Inspector", "ActType", "Organization",
])]
public abstract class BaseAct : BaseActBase, IBaseAct
{
  [StrLen(500)]
  public virtual string? OrganizationName { get; set; }

  [StrLen(250)]
  public virtual string? OrganizationPhone { get; set; }

  [StrLen(250)]
  public virtual string? OrganizationAddress { get; set; }

  [PropertyStorage(["Organization"])]
  public virtual Organization? Organization { get; set; }

  [NotStored]
  [IgnoreDataMember]
  IOrganizationBase? IWOrganization.Organization
  {
    get => Organization;
    set
    {
      if (value is Organization org)
      {
        Organization = org;
      }
    }
  }

  /// <summary>
  /// Class views container.
  /// </summary>
  public new class Views
  {
    /// <summary>
    /// "BaseActE" view.
    /// </summary>
    public static View BaseActE
      => Information.GetView("BaseActE", typeof(BaseAct));
  }
}
