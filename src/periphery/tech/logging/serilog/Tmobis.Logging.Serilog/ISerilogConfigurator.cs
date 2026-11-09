namespace Tmobis.Logging.Serilog
{
    using global::Serilog;
    using Microsoft.Extensions.Configuration;

    public interface ISerilogConfigurator
    {
        void Configure(LoggerConfiguration loggerConfiguration, IConfiguration configuration);
    }
}
