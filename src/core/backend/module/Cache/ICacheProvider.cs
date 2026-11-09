namespace Tmobis.Core.Cache;

/// <summary>
/// Общий контракт платформенного кэша.
/// </summary>
public interface ICacheProvider
{
  /// <summary>
  /// Получить значение по ключу.
  /// </summary>
  /// <typeparam name="T">Тип значения.</typeparam>
  /// <param name="key">Ключ записи.</param>
  /// <returns>Значение из кэша.</returns>
  T? Get<T>(string key);

  /// <summary>
  /// Попробовать получить значение по ключу.
  /// </summary>
  /// <typeparam name="T">Тип значения.</typeparam>
  /// <param name="key">Ключ записи.</param>
  /// <param name="value">Значение из кэша.</param>
  /// <returns>Флаг, найдена ли запись.</returns>
  bool TryGetValue<T>(string key, out T? value);

  /// <summary>
  /// Сохранить значение в кэш.
  /// </summary>
  /// <typeparam name="T">Тип значения.</typeparam>
  /// <param name="key">Ключ записи.</param>
  /// <param name="value">Сохраняемое значение.</param>
  /// <param name="ttl">Время жизни записи.</param>
  void Set<T>(string key, T value, TimeSpan? ttl = null);

  /// <summary>
  /// Получить значение по ключу или создать его при отсутствии.
  /// </summary>
  /// <typeparam name="T">Тип значения.</typeparam>
  /// <param name="key">Ключ записи.</param>
  /// <param name="factory">Фабрика значения.</param>
  /// <param name="ttl">Время жизни записи.</param>
  /// <returns>Значение из кэша или созданное значение.</returns>
  Task<T?> GetOrCreateAsync<T>(
    string key,
    Func<Task<T?>> factory,
    TimeSpan? ttl = null);

  /// <summary>
  /// Удалить запись из кэша.
  /// </summary>
  /// <param name="key">Ключ записи.</param>
  void Remove(string key);
}
