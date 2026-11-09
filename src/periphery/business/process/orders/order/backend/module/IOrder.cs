namespace Tmobis.Business.Process.Orders.Order;

using Core.Models;
using Tmobis.Business.Common.Contacts.Customer.Base;
using Tmobis.Business.Common.Contacts.Supplier.Base;
using Tmobis.Business.Process.Orders.Order.Base;
using Tmobis.Core;

public interface IOrder :
  Base.IOrder,
  IWSupplier,
  IWCustomer
{
  // DetailArray</*I*/OrderTool, /*I*/Order> OrderTools { get; set; }
}
