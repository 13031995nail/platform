namespace Tmobis.Cache.Redis;

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Cache;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль Redis-кэша.
/// Использует Redis как реализацию платформенного <c>ICacheProvider</c>.
/// </summary>
[ModuleRequirement(typeof(CoreModule))]
public sealed class RedisCacheModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);

    services.Configure<RedisOptions>(
      configuration.GetSection(RedisOptions.Redis));

    var redisOptions = GetOptions(configuration);
    services.AddStackExchangeRedisCache(options =>
    {
      options.Configuration = redisOptions.ConnectionString;
      options.InstanceName = redisOptions.InstanceName;
    });
    services.AddOrReplaceTransient<ICacheProvider, RedisCacheProvider>();
  }

  private static RedisOptions GetOptions(IConfiguration configuration)
  {
    var options = new RedisOptions();
    configuration.GetSection(RedisOptions.Redis).Bind(options);
    return options;
  }
}
