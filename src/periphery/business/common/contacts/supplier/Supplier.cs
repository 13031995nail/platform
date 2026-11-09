namespace Tmobis.Business.Common.Contacts.Supplier;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts.Supplier.Base;

[ClassStorage("Supplier")]
public class Supplier :
  SupplierBase,
  ISupplier
{
  [PropertyStorage(["Organization"])]
  public virtual OrganizationBase? Organization { get; set; }


  IOrganizationBase? IWOrganization.Organization
  {
    get => Organization;
    set
    {
      if (value is OrganizationBase organization)
      {
        Organization = organization;
      }
    }
  }
}
