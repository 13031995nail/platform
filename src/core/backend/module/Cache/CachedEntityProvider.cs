namespace Tmobis.Core.Cache;
using Tmobis.Core.Models;
using Microsoft.Extensions.Options;

public class CachedEntityProvider<T>(
  ICacheProvider cacheProvider, IOptions<CacheOptions> options)
: ICachedEntityProvider<T>
where T : IEntity
{
  private readonly ICacheProvider cacheProvider = cacheProvider;
  private readonly CacheOptions options = options.Value;

  /// <summary>
  /// Возвращает сущность из кэша по ее идентификатору.
  /// </summary>
  /// <param name="id">Идентификатор сущности.</param>
  /// <returns>Сущность.</returns>
  public virtual T? GetFromCache(string id)
  => cacheProvider.Get<T>(GetCacheKey(id));

  /// <summary>
  /// Сохраняет сущность в кэш по ее идентификатору.
  /// </summary>
  /// <param name="id">Идентификатор.</param>
  /// <param name="value">Сущность.</param>
  /// <param name="ttl">Время жизни записи.</param>
  public virtual void SetToCache(string id, T value, TimeSpan? ttl = null)
  => cacheProvider.Set(
      GetCacheKey(id), value, ttl ?? options.EntityCacheLifetime);

  /// <summary>
  /// Удалить сущность из кэша.
  /// </summary>
  /// <param name="id">Идентификатор сущности.</param>
  public void RemoveFromCache(string id)
  => cacheProvider.Remove(GetCacheKey(id));

  /// <summary>
  /// Формирует и возвращает ключ сущности, по которому она хранится к кэше.
  /// </summary>
  /// <param name="id">Идентификатор.</param>
  protected virtual string GetCacheKey(string id)
    => $"{nameof(T)}_{id}";
}
