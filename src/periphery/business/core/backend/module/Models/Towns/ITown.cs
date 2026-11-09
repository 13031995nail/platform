namespace Tmobis.Business.Core.Models.Towns;

using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public interface ITown : IDictionary, IRegisteredEntity
{
  public string CityName { get; set; }

  public string CityAcronym { get; set; }

  public int? HourOffset { get; set; }

  public string? Comment { get; set; }
  
  public class Views
  {
      public static readonly EntityView TownE
          = new(
              nameof(TownE),
              searchProperties: [
                  nameof(Name)
              ]);
  }
}
