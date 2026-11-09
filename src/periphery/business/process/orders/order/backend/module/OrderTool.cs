namespace Tmobis.Business.Process.Orders.Order;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Resources.ToolType;
using Tmobis.Frameworks.Flexberry.Entities;

[AutoAltered]
[NotStored(false)]
[ClassStorage("OrderTool")]
[AccessType(AccessType.none)]
[View("OrderToolD", [
  "ToolType"])]
public class OrderTool :
  FlexberryEntity,
  IOrderTool
{
  [PropertyStorage(["ToolType"])]
  [NotNull]
  public virtual /*I*/ToolType ToolType { get; set; }

  [Agregator]
  [NotNull]
  [PropertyStorage(["Order"])]
  public virtual /*I*/Order Order { get; set; }

  IOrder IOrderTool.Order { get => Order; set => throw new NotImplementedException(); }

  /// <summary>
  /// Class views container.
  /// </summary>
  public class Views
  {

    /// <summary>
    /// "OrderToolD" view.
    /// </summary>
    public static ICSSoft.STORMNET.View OrderToolD
    {
      get
      {
        return ICSSoft.STORMNET.Information.GetView("OrderToolD", typeof(OrderTool));
      }
    }
  }
}
