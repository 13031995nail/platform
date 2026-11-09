namespace Tmobis.Logging.Serilog
{
    using global::Serilog;
    using global::Serilog.Events;
    using Microsoft.Extensions.Configuration;

    public class ConsoleConfigurator : ISerilogConfigurator
    {
        public virtual void Configure(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            loggerConfiguration
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }
    }
}
