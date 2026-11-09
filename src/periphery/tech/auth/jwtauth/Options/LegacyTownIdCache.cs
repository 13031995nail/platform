namespace Tmobis.Auth.JwtAuth.Options;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

/// <summary>
/// Кэширует только успешно разрешённые филиалы владельцев старых JWT.
/// Не хранит токены и не подменяет проверку подписи JWT.
/// </summary>
public sealed class LegacyTownIdCache(
    IMemoryCache cache,
    IOptions<JwtAuthOptions> options)
{
    public async Task<Guid?> ResolveAsync(
        string login,
        Func<string, Task<Guid?>> loadTownId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        ArgumentNullException.ThrowIfNull(loadTownId);

        var key = $"legacy-jwt:town:{login}";
        if (cache.TryGetValue(key, out Guid cachedTownId))
            return cachedTownId;

        var townId = await loadTownId(login);
        var lifetime = options.Value.LegacyTownCacheLifetime;
        if (townId is { } resolved && resolved != Guid.Empty &&
            lifetime > TimeSpan.Zero)
        {
            cache.Set(key, resolved, lifetime);
        }

        return townId;
    }
}
