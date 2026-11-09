namespace Tmobis.Business.Common.Contacts.Customer;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts.Customer.Base;

[ClassStorage("Customer")]
public class Customer :
  CustomerBase,
  ICustomer
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
