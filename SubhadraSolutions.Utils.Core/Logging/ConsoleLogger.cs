using Microsoft.Extensions.Logging;

namespace SubhadraSolutions.Utils.Logging;

public class ConsoleLogger : AbstractLoggerBase
{
    private readonly bool useColors;

    public ConsoleLogger(bool logAsJson, bool useColors, params LogLevel[] enabledLogLevels) :
        base(logAsJson, enabledLogLevels)
    {
        this.useColors = useColors;
    }

    protected override void Log(LogLevel logLevel, string message)
    {
        ConsoleHelper.Log(logLevel, message, this.useColors);
    }
}