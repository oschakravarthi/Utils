using Microsoft.Extensions.Logging;
using System;

namespace SubhadraSolutions.Utils.Logging
{
    public class PublishingLogger : AbstractLoggerBase
    {
        public PublishingLogger(bool logAsJson, params LogLevel[] enabledLogLevels) : base(logAsJson, enabledLogLevels)
        {
        }

        public event Action<LogLevel, string> OnLog;

        protected override void Log(LogLevel logLevel, string message)
        {
            OnLog?.Invoke(logLevel, message);
        }
    }
}