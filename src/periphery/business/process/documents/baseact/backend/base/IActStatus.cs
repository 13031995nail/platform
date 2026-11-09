namespace Tmobis.Business.Process.Documents;
using Tmobis.Core.Models;

public interface IActStatus : IDictionary, IRegisteredEntity
{
  bool DeleteProtected { get; set; }

  /// <summary>
  /// Статические ключи сущностей
  /// </summary>
  public static class Keys
  {
    /// <summary>
    /// Создан
    /// </summary>
    public static Guid Created { get; } = new ("119719b2-e3bd-4f00-a051-2426220d3050");

    /// <summary>
    /// На редактировании
    /// </summary>
    public static Guid Editing { get; } = new ("119719b2-e3bd-4f00-a051-2426220d3051");
    
    /// <summary>
    /// На подписании
    /// </summary>
    public static Guid Signing { get; } = new ("119719b2-e3bd-4f00-a051-2426220d3052");

    /// <summary>
    /// Подписан
    /// </summary>
    public static Guid Signed { get; } = new ("119719b2-e3bd-4f00-a051-2426220d3053");

    /// <summary>
    /// Утвержден
    /// </summary>
    public static Guid Approved { get; } = new ("119719b2-e3bd-4f00-a051-2426220d3054");
  }
}