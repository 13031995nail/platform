namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

public class WorkObjectBase : FlexberryActualEntity
{
  [StrLen(500)]
  [NotNull]
  public virtual string Name { get; set; }

  [StrLen(350)]
  [NotNull]
  public virtual string AddressString { get; set; }

  public virtual IWorkObject? Hierarchy { get; set; }
}
