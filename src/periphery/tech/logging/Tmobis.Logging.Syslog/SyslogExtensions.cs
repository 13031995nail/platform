namespace Tmobis.Logging.Syslog
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Расширение для ILoggingBuilder, добавляющее провайдер для Syslog.
    /// </summary>
    public static class SyslogLoggerExtensions
    {
        /// <summary>
        /// Преобразовать значение в SQL строку.
        /// </summary>
        /// <param name="builder">ILoggingBuilder.</param>
        /// <param name="config">Из конфига.</param>
        /// <returns>Провайдер для Syslog-логгера.</returns>
        public static ILoggingBuilder AddSyslog(
            this ILoggingBuilder builder,
            IConfigurationSection config)
        {
            var options = new SyslogOptions();
            config.Bind(options);  // Биндим JSON настройки в опции
            builder.AddProvider(new SyslogLoggerProvider(options));
            return builder;
        }
    }
}
