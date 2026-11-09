namespace Tmobis.Cache.Redis;

/// <summary>
/// Настройки подключения к Redis.
/// </summary>
public class RedisOptions
{
  public const string Redis = "Redis";

  /// <summary>
  /// Строка подключения к Redis.
  /// </summary>
  public string ConnectionString { get; set; } = string.Empty;

  /// <summary>
  /// Префикс ключей приложения в Redis.
  /// </summary>
  public string InstanceName { get; set; } = string.Empty;
}
