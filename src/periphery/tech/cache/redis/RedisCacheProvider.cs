namespace Tmobis.Cache.Redis;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tmobis.Core.Cache;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Mappers;

/// <summary>
/// Провайдер кэша на основе Redis.
/// </summary>
public class RedisCacheProvider(
  IDistributedCache cache,
  IEntityMaterializer entityMaterializer,
  IMessageTypeBinder messageTypeBinder,
  ILogger<RedisCacheProvider> logger) : ICacheProvider
{
  private readonly IDistributedCache cache = cache;
  private readonly RedisCacheSerializer serializer = new(
    entityMaterializer,
    messageTypeBinder);
  private readonly ILogger<RedisCacheProvider> logger = logger;

  /// <inheritdoc/>
  public T? Get<T>(string key)
  {
    var bytes = cache.Get(key);
    return bytes != null && TryDeserialize(key, bytes, out T? value)
      ? value
      : default;
  }

  /// <inheritdoc/>
  public bool TryGetValue<T>(string key, out T? value)
  {
    var bytes = cache.Get(key);
    if (bytes == null)
    {
      value = default;
      return false;
    }

    return TryDeserialize(key, bytes, out value);
  }

  /// <inheritdoc/>
  public void Set<T>(string key, T value, TimeSpan? ttl = null)
    => cache.Set(key, serializer.Serialize(value), CreateEntryOptions(ttl));

  /// <inheritdoc/>
  public async Task<T?> GetOrCreateAsync<T>(
    string key,
    Func<Task<T?>> factory,
    TimeSpan? ttl = null)
  {
    ArgumentNullException.ThrowIfNull(factory);

    var bytes = await cache.GetAsync(key);
    if (bytes != null && TryDeserialize(key, bytes, out T? cachedValue))
    {
      return cachedValue;
    }

    var createdValue = await factory();
    if (createdValue is not null)
    {
      await cache.SetAsync(
        key,
        serializer.Serialize(createdValue),
        CreateEntryOptions(ttl));
    }

    return createdValue;
  }

  /// <inheritdoc/>
  public void Remove(string key)
    => cache.Remove(key);

  private bool TryDeserialize<T>(string key, byte[] bytes, out T? value)
  {
    try
    {
      var deserialized = serializer.Deserialize<T>(bytes);
      if (deserialized is not T typedValue)
      {
        value = default;
        return false;
      }

      value = typedValue;
      return true;
    }
    catch (Exception ex) when (IsInvalidCachedPayload(ex))
    {
      logger.LogWarning(
        ex,
        "Некорректная запись Redis-кэша будет удалена. Key: {CacheKey}",
        key);
      cache.Remove(key);
      value = default;
      return false;
    }
  }

  private static DistributedCacheEntryOptions CreateEntryOptions(
    TimeSpan? ttl)
  {
    var options = new DistributedCacheEntryOptions();
    if (ttl.HasValue)
    {
      options.AbsoluteExpirationRelativeToNow = ttl.Value;
    }

    return options;
  }

  private static bool IsInvalidCachedPayload(Exception exception)
    => exception is JsonException
      || exception is InvalidOperationException
      || exception is ArgumentException;
}
