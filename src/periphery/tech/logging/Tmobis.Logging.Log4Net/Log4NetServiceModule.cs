namespace Tmobis.Logging.Log4Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tmobis.Core.Modules;
using Microsoft.Extensions.Logging;

/// <summary>
/// Модуль логирования через Log4Net.
/// Использует конфигурацию <c>log4net.config</c>.
/// </summary>
public sealed class Log4NetServiceModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddLogging(loggingBuilder =>
    {
      // TODO: Не хочется чистить вывод в консоль и журнал
      // loggingBuilder.ClearProviders(); // Clear default providers
      loggingBuilder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "log4net.config"));
    });
  }
}
