namespace Tmobis.Logging.Syslog
{
    public class SyslogOptions
    {
        public const string Syslog = "Syslog";
        public string Server { get; set; } = string.Empty;

        public int Port { get; set; } = 0;

        public int Facility { get; set; }

        public int Version { get; set; }

        public string AppName { get; set; } = string.Empty;
    }
}
