namespace Tmobis.Core.Cache;
using Tmobis.Core.Models;

public interface ICachedEntityProvider<T>
where T : IEntity
{
  /// <summary>
  /// Возвращает сущность из кэша по ее идентификатору.
  /// </summary>
  /// <param name="id">Идентификатор сущности.</param>
  /// <returns>Сущность.</returns>
  public T? GetFromCache(string id);

  /// <summary>
  /// Сохраняет сущность в кэш по ее идентификатору.
  /// </summary>
  /// <param name="id">Идентификатор.</param>
  /// <param name="value">Сущность.</param>
  /// <param name="ttl">Время жизни записи.</param>
  public void SetToCache(string id, T value, TimeSpan? ttl = null);

  /// <summary>
  /// Удалить сущность из кэша.
  /// </summary>
  /// <param name="id">Идентификатор сущности.</param>
  public void RemoveFromCache(string id);
}
