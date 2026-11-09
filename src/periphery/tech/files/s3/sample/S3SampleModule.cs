namespace Tmobis.Files.Adapter.S3.Sample;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Core.Repositories;

internal sealed class S3SampleModule : ModuleBase
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);

    services.Configure<S3SampleOptions>(
      configuration.GetSection(S3SampleOptions.SectionName));
    services.AddSingleton<InMemoryFileRepository>();
    services.AddSingleton<IReadRepository>(
      provider => provider.GetRequiredService<InMemoryFileRepository>());
    services.AddSingleton<IWriteRepository>(
      provider => provider.GetRequiredService<InMemoryFileRepository>());
    services.AddSingleton<IRepository>(
      provider => provider.GetRequiredService<InMemoryFileRepository>());
    services.AddSingleton<IUnitOfWork>(
      provider => provider.GetRequiredService<InMemoryFileRepository>());
    services.AddHostedService<S3SampleWorker>();
  }
}
