namespace Tmobis.Business.Common.Contacts.Customer.Base;

using ICSSoft.STORMNET;

public interface IWCustomer
{
  [PropertyStorage(["Customer"])]
  public /*I*/CustomerBase? Customer { get; set; }
}
