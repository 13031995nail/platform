namespace Tmobis.Business.Common.Address;
using Tmobis.Core.Models;

public interface IWorkObject : IHierarchialEntity<IWorkObject>, IActualEntity
{
  public string Name { get; set; }

  public string AddressString { get; set; }
}
