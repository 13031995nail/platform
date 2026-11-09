using Microsoft.Extensions.DependencyInjection;
using Tmobis.Cache.Redis;
using Tmobis.Cache.Redis.Sample;
using Tmobis.Core.Application;
using Tmobis.Core.Cache;
using Tmobis.Core.Modules;

var modules = new ModuleRegisterBuilder([
  typeof(RedisCacheModule),
]);

using var app = new WorkerAppBuilder(modules, args).Build();

var cacheProvider = app.Services.GetRequiredService<ICacheProvider>();
var cacheKey = $"redis-sample:{Guid.NewGuid():N}";
var record = new RedisSampleRecord(
  Id: Guid.NewGuid().ToString("N"),
  Value: "Hello from stands.timepath.ru Redis");

cacheProvider.Set(cacheKey, record, TimeSpan.FromMinutes(5));
var restoredRecord = cacheProvider.Get<RedisSampleRecord>(cacheKey);

Console.WriteLine($"Key: {cacheKey}");
Console.WriteLine($"Stored: {record}");
Console.WriteLine($"Restored: {restoredRecord}");
