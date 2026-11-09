namespace Tmobis.Core.Models;

public interface IHierarchialEntity<T> : IEntity
where T : IEntity
{
  public T? Hierarchy { get; set; }
}