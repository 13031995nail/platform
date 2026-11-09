namespace Tmobis.Business.Common.Contacts.Supplier.Base;

using ICSSoft.STORMNET;

public interface IWSupplier
{
  [PropertyStorage(["Supplier"])]
  public /*I*/SupplierBase? Supplier { get; set; }
}
