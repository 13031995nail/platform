namespace Tmobis.Business.Common.Contacts;

using ICSSoft.STORMNET;
using Tmobis.Frameworks.Flexberry.Entities;

public class OrganizationBase :
  FlexberryActualEntity,
  IOrganizationBase
{
  [StrLen(200)]
  [NotNull]
  public virtual string Name { get; set; }

  [StrLen(350)]
  public virtual string Address { get; set; }

  [StrLen(350)]
  public virtual string LegalAddress { get; set; }

  [StrLen(50)]
  public virtual string Phone { get; set; }

  [StrLen(250)]
  public virtual string Email { get; set; }

  [StrLen(10)]
  public virtual string Acronym { get; set; }
}
