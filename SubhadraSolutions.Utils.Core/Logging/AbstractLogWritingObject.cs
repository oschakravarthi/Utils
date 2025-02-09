using Microsoft.Extensions.Logging;
using System;

namespace SubhadraSolutions.Utils.Logging;

public class AbstractLogWritingObject
{
    protected readonly ILogger logger;

    protected AbstractLogWritingObject(ILogger logger)
    {
        this.logger = logger;
    }

    protected void LogException(Exception ex)
    {
        if (logger == null || ex == null)
        {
            return;
        }

        logger.Log(LogLevel.Error, ex, ex.Message);
    }
}