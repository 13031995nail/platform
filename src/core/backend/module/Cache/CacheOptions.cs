namespace Tmobis.Core.Cache;

public class CacheOptions
{
    public const string Cache = "Cache";

    public TimeSpan EntityCacheLifetime { get; set; }
    = TimeSpan.FromHours(1);
}