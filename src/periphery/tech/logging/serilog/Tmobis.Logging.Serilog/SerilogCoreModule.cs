namespace Tmobis.Logging.Serilog;

using global::Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.DI;
using Tmobis.Core.Modules;

/// <summary>
/// Модуль логирования через Serilog.
/// Содержит дефолтный конфигуратор через консоль
/// и применяет зарегистрированные <c>ISerilogConfigurator</c>.
/// </summary>
public class SerilogCoreModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.AddTransient<ISerilogConfigurator, ConsoleConfigurator>();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.Services.AddSerilog((serviceProvider, loggerConfiguration) =>
            {
                var configurators = serviceProvider.GetServices<ISerilogConfigurator>();
                foreach (var configurator in configurators)
                {
                    configurator.Configure(loggerConfiguration, configuration);
                }
            });
        });
    }
}
