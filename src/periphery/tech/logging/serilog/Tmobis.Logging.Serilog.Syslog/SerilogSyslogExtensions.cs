namespace Tmobis.Logging.Serilog.Syslog;

using global::Serilog;
using global::Serilog.Configuration;
using global::Serilog.Sinks.Syslog;
using Microsoft.Extensions.Configuration;

public static class SerilogSyslogExtensions
{
    public static LoggerConfiguration Syslog(this LoggerSinkConfiguration sinkConfiguration,
        IConfiguration configuration)
    {
        var options = new SerilogSyslogOptions();
        configuration.GetSection(SerilogSyslogOptions.Syslog).Bind(options);

        return Syslog(sinkConfiguration, options);
    }

    public static LoggerConfiguration Syslog(this LoggerSinkConfiguration sinkConfiguration,
        SerilogSyslogOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Server) || options.Port <= 0)
        {
            Console.WriteLine("Syslog server not configured");
            return sinkConfiguration.Sink(new NullSink());
        }

        var facility = (Facility)options.Facility;

        var syslogFormat = options.Format == SyslogFormat.RFC3164
            ? global::Serilog.Sinks.Syslog.SyslogFormat.RFC3164
            : global::Serilog.Sinks.Syslog.SyslogFormat.RFC5424;

        return options.Protocol switch
        {
            ProtocolType.Tcp when options.SecureTcp => sinkConfiguration.TcpSyslog(
                host: options.Server,
                port: options.Port,
                appName: options.AppName,
                framingType: FramingType.OCTET_COUNTING,
                format: syslogFormat,
                facility: facility,
                useTls: true),

            ProtocolType.Tcp => sinkConfiguration.TcpSyslog(
                host: options.Server,
                port: options.Port,
                appName: options.AppName,
                framingType: FramingType.OCTET_COUNTING,
                format: syslogFormat,
                facility: facility,
                useTls: false),

            ProtocolType.Udp => sinkConfiguration.UdpSyslog(
                host: options.Server,
                port: options.Port,
                appName: options.AppName,
                format: syslogFormat,
                facility: facility),

            _ => sinkConfiguration.TcpSyslog(
                host: options.Server,
                port: options.Port,
                appName: options.AppName,
                format: syslogFormat,
                facility: facility)
        };
    }
}

public class NullSink : global::Serilog.Core.ILogEventSink
{
    public void Emit(global::Serilog.Events.LogEvent logEvent)
    {
    }
}
