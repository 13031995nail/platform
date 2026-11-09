namespace Tmobis.Core.Models;

public abstract class Dictionary
: ActualEntity, IDictionary
{
  /// <inheritdoc/>
  public virtual string? Code { get; set; }
  
  /// <inheritdoc/>
  public virtual string Name { get; set; } = string.Empty;
}
