using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Runtime;
using System;

namespace SubhadraSolutions.Utils;

public static class ConsoleHelper
{
    private static readonly object synclock = new object();

    public static void Log(LogLevel logLevel, string message, bool useColors)
    {
        if (!useColors || RuntimeHelper.IsBrowserApplication)
        {
            Console.Write(message);
            Console.WriteLine();
            return;
        }

        var previousForegroundColor = ConsoleColor.White;
        var previousBackgroundColor = ConsoleColor.Black;

        var currentForegroundColor = ConsoleColor.White;
        var currentBackgroundColor = ConsoleColor.Black;
        switch (logLevel)
        {
            case LogLevel.Critical:
                currentForegroundColor = ConsoleColor.White;
                currentBackgroundColor = ConsoleColor.Red;
                break;

            case LogLevel.Debug:
                currentForegroundColor = ConsoleColor.White;
                break;

            case LogLevel.Information:
                currentForegroundColor = ConsoleColor.Blue;
                break;

            case LogLevel.Warning:
                currentForegroundColor = ConsoleColor.Yellow;
                break;

            case LogLevel.Error:
                currentForegroundColor = ConsoleColor.Red;
                break;

            case LogLevel.Trace:
                currentForegroundColor = ConsoleColor.Gray;
                break;
        }

        lock (synclock)
        {
            Console.ForegroundColor = currentForegroundColor;
            Console.BackgroundColor = currentBackgroundColor;

            Console.Write(message);
            Console.ForegroundColor = previousForegroundColor;
            Console.BackgroundColor = previousBackgroundColor;

            Console.WriteLine();
        }
    }
}