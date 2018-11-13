using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace AfxDotNetCoreSample.Common
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        private SqlLogger logger;
        public ILogger CreateLogger(string categoryName)
        {
            if (this.logger == null) this.logger = new SqlLogger();

            return this.logger;
        }

        public void Dispose()
        {
            this.logger = null;
        }
    }

    public class SqlLogger : ILogger, IDisposable
    {
        public const string LOG_NAME = "SQL";
        private log4net.ILog _log;
        private log4net.ILog log
        {
            get
            {
                if (this._log == null) this._log = Common.LogUtils.GetLog(LOG_NAME);

                return this._log;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return this.log.IsDebugEnabled;
                case LogLevel.Information:
                    return this.log.IsInfoEnabled;
                case LogLevel.Warning:
                    return this.log.IsWarnEnabled;
                case LogLevel.Error:
                    return this.log.IsErrorEnabled;
                case LogLevel.Critical:
                    return this.log.IsFatalEnabled;
                default:
                    return !(this.log.IsFatalEnabled || this.log.IsErrorEnabled || this.log.IsWarnEnabled || this.log.IsInfoEnabled || this.log.IsDebugEnabled);
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string msg = formatter(state, exception);
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    this.log.Debug(msg);
                    break;
                case LogLevel.Information:
                    this.log.Info(msg);
                    break;
                case LogLevel.Warning:
                    this.log.Warn(msg);
                    break;
                case LogLevel.Error:
                    this.log.Error(msg);
                    break;
                case LogLevel.Critical:
                    this.log.Fatal(msg);
                    break;
                default:

                    break;
            }
        }

        public void Dispose()
        {

        }
    }
}
