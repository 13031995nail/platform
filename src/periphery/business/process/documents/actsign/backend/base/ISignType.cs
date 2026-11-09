namespace Tmobis.Business.Process.Documents;
using Tmobis.Core.Models;

public interface ISignType : IDictionary
{
  /// <summary>
  /// Статические ключи сущностей
  /// </summary>
  public static class Keys
  {
    /// <summary>
    /// Создан
    /// </summary>
    public static Guid Created { get; } = new ("119719b2-e3bd-4f00-a051-2426220d3050");

  }
}
