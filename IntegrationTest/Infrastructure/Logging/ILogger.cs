using System;

namespace IntegrationTest.Infrastructure
{
    public interface ILogger
    {
        void Error(Exception exception);
        void Error(string format, params object[] args);
        void Error(Exception exception, string format, params object[] args);
        void Warning(string format, params object[] args);
        void Info(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Trace(Exception exception);
        void Trace(string format, params object[] args);
    }
}