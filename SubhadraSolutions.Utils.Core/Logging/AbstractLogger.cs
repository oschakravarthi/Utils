using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Logging;

public abstract class AbstractLogger : ILogger
{
    private readonly HashSet<LogLevel> enabledLogLevels;

    protected AbstractLogger(params LogLevel[] enabledLogLevels)
    {
        if (enabledLogLevels != null)
        {
            this.enabledLogLevels = new HashSet<LogLevel>(enabledLogLevels);
        }
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (enabledLogLevels == null || enabledLogLevels.Count == 0)
        {
            return true;
        }

        return enabledLogLevels.Contains(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        if (logLevel == LogLevel.None)
        {
            return;
        }

        if (!IsEnabled(logLevel))
        {
            return;
        }

        LogCore(logLevel, eventId, state, exception, formatter);
    }

    protected abstract void LogCore<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter);
}