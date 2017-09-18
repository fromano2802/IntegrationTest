using System;
using NLog;

namespace IntegrationTest.Infrastructure
{
    public class NLogProxy<T> : ILogger
    {
        private static readonly NLog.ILogger Logger = 
            LogManager.GetLogger(typeof (T).FullName);

        public void Error(Exception exception)
        {
            Logger.Log(LogLevel.Error, exception);
        }

        public void Error(string format, params object[] args)
        {
            Logger.Log(LogLevel.Error, format, args);
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            Logger.Log(LogLevel.Error, exception, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            Logger.Log(LogLevel.Warn, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Logger.Log(LogLevel.Info, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            Logger.Log(LogLevel.Debug, format, args);
        }

        public void Trace(Exception exception)
        {
            Logger.Log(LogLevel.Trace, exception);
        }

        public void Trace(string format, params object[] args)
        {
            Logger.Log(LogLevel.Trace, format, args);
        }
    }
}