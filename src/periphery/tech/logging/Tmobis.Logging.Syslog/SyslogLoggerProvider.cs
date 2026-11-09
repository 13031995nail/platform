namespace Tmobis.Logging.Syslog
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;

    public class SyslogLoggerProvider : ILoggerProvider
    {
        private readonly SyslogOptions options;
        public SyslogLoggerProvider(SyslogOptions opt)
        {
            options = opt;
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            //LogService.LogInfo("Инициирован кастомный логгер - " + categoryName);

            return new SyslogLogger(options);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
        }
    }
}
