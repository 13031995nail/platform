namespace Tmobis.Logging.Serilog.Syslog;

public class SerilogSyslogOptions
{
    public const string Syslog = "Syslog";

    public string Server { get; set; } = string.Empty;

    public int Port { get; set; } = 514;

    public int Facility { get; set; } = 16;

    public string AppName { get; set; } = "Tmobis";

    public SyslogFormat Format { get; set; } = SyslogFormat.RFC5424;

    public ProtocolType Protocol { get; set; } = ProtocolType.Tcp;

    public bool SecureTcp { get; set; } = false;

    public string OutputTemplate { get; set; } = "{Message}";
}

public enum SyslogFormat
{
    RFC3164,
    RFC5424
}

public enum ProtocolType
{
    Tcp,
    Udp
}
