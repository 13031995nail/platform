namespace Tmobis.Logging.Syslog
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class SyslogLogger : ILogger
    {
        private readonly SyslogOptions options;
        public SyslogLogger(SyslogOptions opt)
        {
            options = opt;
        }

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{logLevel}: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            var syslogLevel = MapToSyslogLevel(logLevel);
            Send(syslogLevel, message);

        }

        /// <summary>
        /// Отправить сообщение на syslog-сервер.
        /// </summary>
        /// <param name="logLevel">Уровень сообщения.</param>
        /// <param name="message">Сообщение.</param>
        private void Send(SyslogLogLevel logLevel, string message)
        {
            if (string.IsNullOrWhiteSpace(options.Server) || options.Port <= 0)
            {
                return;
            }

            /*
             * Пример SysLog сообщения в формате RFC 5424
             * <165>1 2003-08-24T05:14:15.000003-07:00 192.0.2.1 myproc 8710 - - TestMessage1
             *
             * <165> - приоритет, расичтывается как 8 * Facility(код источника) + Severity(важность).
             * 1 - Version
             * 2003-08-24T05:14:15.000003-07:00 - Дата-Время.Тип Timestamp
             * 192.0.2.1 - хост источника сообщения.
             * myproc - APP-NAME.
             * 8710 - PROCID.
             * STRUCTURED-DATA, что должно идти после PROCID в данном примере нет, поэтому -
             * Идентификатора сообщения (MSGID), что должен идти после STRUCTURED-DATA в данном примере нет, поэтому -
             * Само сообщение.
             */

            int priority = options.Facility * 8 + (int)logLevel;
            int versionInfo = options.Version;
            string dateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");
            string hostName = Dns.GetHostName();
            string appNameInfo = options.AppName;
            int procIdInfo = 1;

            string logMessage = $"<{priority}>{versionInfo} {dateTime} {hostName} {appNameInfo} {procIdInfo} - - {message}";
            logMessage += Environment.NewLine;

            var bytes = Encoding.UTF8.GetBytes(logMessage);

            using (TcpClient client = new TcpClient(options.Server, options.Port))
            {
                NetworkStream stream = client.GetStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }

        /// <summary>
        /// Преобразовывает уровень сообщения стандартного логгера в уровень syslog.
        /// </summary>
        /// <param name="level">Уровень сообщения в стандартном логгере.</param>
        /// <returns>Уровень сообщения в протоколе syslog.</returns>
        private SyslogLogLevel MapToSyslogLevel(LogLevel level)
        {
            SyslogLogLevel syslogLevel = SyslogLogLevel.Info;

            switch (level)
            {
                case LogLevel.Debug:
                    syslogLevel = SyslogLogLevel.Debug;
                    break;
                case LogLevel.Warning:
                    syslogLevel = SyslogLogLevel.Warn;
                    break;
                case LogLevel.Error:
                    syslogLevel = SyslogLogLevel.Error;
                    break;
                case LogLevel.Critical:
                    syslogLevel = SyslogLogLevel.Critical;
                    break;
                case LogLevel.Trace:
                case LogLevel.None:
                case LogLevel.Information:
                    syslogLevel = SyslogLogLevel.Info;
                    break;
            }

            return syslogLevel;
        }

    }
}
