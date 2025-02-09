using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DummyConsoleApp;

internal class DummyLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        Console.WriteLine(JsonConvert.SerializeObject(state));
    }
}