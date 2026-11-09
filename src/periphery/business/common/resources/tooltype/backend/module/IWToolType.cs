namespace Tmobis.Business.Common.Resources.ToolType;

using ICSSoft.STORMNET;

public interface IWToolType
{
  [PropertyStorage(["ToolType"])]
  [NotNull]
  public /*I*/ToolType ToolType { get; set; }
}
