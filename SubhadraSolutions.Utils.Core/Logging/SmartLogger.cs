using Microsoft.Extensions.Logging;
using System;

namespace SubhadraSolutions.Utils.Logging;

public class SmartLogger : AbstractLogger, ISmartLogger
{
    private readonly IItemWriter<LogItem> logItemWriter;
    private readonly ProductInfo productInfo;

    public SmartLogger(IItemWriter<LogItem> logItemWriter, ProductInfo productInfo) : this(logItemWriter,
        productInfo, null)
    {
    }

    public SmartLogger(IItemWriter<LogItem> logItemWriter, ProductInfo productInfo,
        params LogLevel[] enabledLogLevels)
        : base(enabledLogLevels)
    {
        this.logItemWriter = logItemWriter;
        this.productInfo = productInfo;
    }

    public event EventHandler<GenericEventArgs<Exception>> OnExceptionLogged;

    protected override void LogCore<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        string exceptionAsString = null;
        if (exception != null)
        {
            exceptionAsString = formatter == null ? exception.ToDetailedString() : formatter(state, exception);
        }

        var logItem = new LogItem
        {
            EventId = eventId.Id,
            EventName = eventId.Name,
            Exception = exceptionAsString,
            Message = exception == null ? null : exception.Message,
            LogLevel = logLevel.ToString(),
            ProductName = $"{(productInfo.ShortName ?? productInfo.Name)}-{(productInfo.IsServer ? "Server" : "Client")}",
            ProductVersion = productInfo.Version
        };
        logItemWriter.Write(logItem);

        if (exception != null)
        {
            OnExceptionLogged?.Invoke(this, new GenericEventArgs<Exception>(exception));
        }
    }
}