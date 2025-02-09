using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Abstractions;

namespace SubhadraSolutions.Utils.Logging;

public class LoggerProvider : AbstractDisposable, ILoggerProvider
{
    private readonly ILogger logger;

    public LoggerProvider(ILogger logger)
    {
        this.logger = logger;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return logger;
    }

    protected override void Dispose(bool disposing)
    {
    }
}