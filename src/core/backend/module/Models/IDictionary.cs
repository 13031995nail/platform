namespace Tmobis.Core.Models;

public interface IDictionary : IActualEntity
{
  // [StrLen(10)]
  public string? Code { get; set; }

  // [StrLen(200)]
  // [NotNull()]
  public string Name { get; set; }
}