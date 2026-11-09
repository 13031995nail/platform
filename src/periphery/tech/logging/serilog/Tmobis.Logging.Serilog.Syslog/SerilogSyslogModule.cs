namespace Tmobis.Logging.Serilog.Syslog;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Modules;
using Tmobis.Logging.Serilog;

/// <summary>
/// Модуль Syslog для Serilog.
/// </summary>
public class SerilogSyslogModule : ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<SerilogSyslogOptions>(configuration.GetSection(SerilogSyslogOptions.Syslog));

        services.AddTransient<ISerilogConfigurator, SyslogConfigurator>();
    }
}
