namespace Tmobis.Logging.Syslog
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Tmobis.Core.Modules;

/// <summary>
/// Модуль логирования в Syslog напрямую через AddLogging.
/// </summary>
    public class SyslogModule : ModuleBase
    {
        public override void Register(
            IServiceCollection services,
            IConfiguration configuration,
            IModuleRegister modules)
        {
            base.Register(services, configuration, modules);
            var syslogConfig = configuration.GetSection(SyslogOptions.Syslog);

            // Настраиваем логирование
            services.AddLogging(builder =>
            {
                builder.AddSyslog(syslogConfig);
            });
        }
    }
}
