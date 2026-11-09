namespace Tmobis.Core.Cache;

using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Провайдер кэша на основе <see cref="IMemoryCache"/>.
/// </summary>
public class MemoryCacheProvider(IMemoryCache memoryCache) : ICacheProvider
{
  private readonly IMemoryCache memoryCache = memoryCache;

  /// <inheritdoc/>
  public T? Get<T>(string key)
    => memoryCache.Get<T>(key);

  /// <inheritdoc/>
  public bool TryGetValue<T>(string key, out T? value)
    => memoryCache.TryGetValue(key, out value);

  /// <inheritdoc/>
  public void Set<T>(string key, T value, TimeSpan? ttl = null)
  {
    if (ttl.HasValue)
    {
      memoryCache.Set(key, value, ttl.Value);
      return;
    }

    memoryCache.Set(key, value);
  }

  /// <inheritdoc/>
  public async Task<T?> GetOrCreateAsync<T>(
    string key,
    Func<Task<T?>> factory,
    TimeSpan? ttl = null)
  {
    ArgumentNullException.ThrowIfNull(factory);

    if (TryGetValue(key, out T? cachedValue))
    {
      return cachedValue;
    }

    var createdValue = await factory();
    if (createdValue is not null)
    {
      Set(key, createdValue, ttl);
    }

    return createdValue;
  }

  /// <inheritdoc/>
  public void Remove(string key)
    => memoryCache.Remove(key);
}
