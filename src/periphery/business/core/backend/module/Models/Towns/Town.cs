namespace Tmobis.Business.Core.Models.Towns;
using Tmobis.Core.Models;

public class Town : Dictionary, ITown
{
  /// <inheritdoc/>
  public virtual int? HourOffset { get; set; }
  
  /// <inheritdoc/>
  public virtual string CityName { get; set; }

  /// <inheritdoc/>
  public virtual string CityAcronym { get; set; }
  
  /// <inheritdoc/>
  public virtual string? Comment { get; set; }
}
