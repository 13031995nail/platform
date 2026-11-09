
namespace Tmobis.Business.Common.Resources.Units;

using ICSSoft.STORMNET;
using Tmobis.Core;

public interface IWUnit
{
  [PropertyStorage(["Unit"])]
  [NotNull]
  public IUnit Unit { get; set; }
}