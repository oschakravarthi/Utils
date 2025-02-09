using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace SubhadraSolutions.Utils.Logging
{
    public abstract class AbstractLoggerBase : AbstractLogger
    {
        protected readonly bool logAsJson;

        public AbstractLoggerBase(bool logAsJson, params LogLevel[] enabledLogLevels) :
            base(enabledLogLevels)
        {
            this.logAsJson = logAsJson;
        }

        protected abstract void Log(LogLevel logLevel, string message);

        protected override void LogCore<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
        {
            var applicableLogLevel = logLevel;
            if (exception != null && logLevel is LogLevel.Debug or LogLevel.Information or LogLevel.Warning or LogLevel.Trace)
            {
                applicableLogLevel = LogLevel.Error;
            }

            string exceptionAsString = null;
            if (exception != null)
            //exceptionAsString = formatter == null ? exception.ToDetailedString() : formatter(state, exception);
            {
                exceptionAsString = exception.ToDetailedString();
            }
            string message = null;
            if (logAsJson)
            {
                var jObject = new JObject
                {
                    ["Timestamp"] = GlobalSettings.Instance.DateTimeNow.ToString(GlobalSettings.Instance.DateAndTimeSerializationFormat),
                    ["LogLevel"] = logLevel.ToString(),
                    ["EventId"] = eventId.ToString(),
                    ["State"] = JToken.FromObject(state),
                    ["Exception"] = exceptionAsString
                };
                message = jObject.ToString();
            }
            else
            {
                message = state == null ? null : state.ToString();
                if (exception != null)
                {
                    message += Environment.NewLine + exception.ToString();
                }
            }
            Log(applicableLogLevel, message);
        }
    }
}