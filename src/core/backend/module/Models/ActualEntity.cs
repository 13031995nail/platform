namespace Tmobis.Core.Models;

public abstract class ActualEntity
: BusinessEntity, IActualEntity
{
  /// <inheritdoc/>
  public virtual bool Actuality { get; set; } = true;
}
