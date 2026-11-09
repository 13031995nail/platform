namespace Tmobis.Logging.Serilog.Syslog;

using global::Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using Tmobis.Logging.Serilog;

public class SyslogConfigurator(IOptions<SerilogSyslogOptions> syslogOptions) : ISerilogConfigurator
{
    private readonly SerilogSyslogOptions syslogOptions = syslogOptions.Value;
    public virtual void Configure(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        if (syslogOptions != null && !string.IsNullOrWhiteSpace(syslogOptions.Server) && syslogOptions.Port > 0)
        {
            loggerConfiguration.WriteTo.Syslog(configuration);
        }
        else
        {
            Console.WriteLine("Syslog is not configured correctly in config");
        }
    }
}
