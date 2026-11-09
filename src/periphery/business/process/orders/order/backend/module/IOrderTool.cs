namespace Tmobis.Business.Process.Orders.Order;

using Core.Models;
using ICSSoft.STORMNET;
using Tmobis.Business.Common.Resources.ToolType;
using Tmobis.Core;
using Tmobis.Core.Models;

public interface IOrderTool :
  IEntity,
  IWToolType
{
  [Agregator]
  [NotNull]
  [PropertyStorage(["Order"])]
  IOrder Order { get; set; }
}
