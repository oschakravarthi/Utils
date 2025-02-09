using SubhadraSolutions.Utils.Diagnostics;
using System;

namespace SubhadraSolutions.Utils.Console;

public class ConsoleOutput
{
    public static bool Print(string format, params object[] fields)
    {
        return Print_helper(Verbosity.Default, format, false, fields);
    }

    public static bool Print(Verbosity verbosity, string format, params object[] fields)
    {
        return Print_helper(verbosity, format, false, fields);
    }

    public static bool Print(string message = null)
    {
        message ??= string.Empty;
        return Print_helper(Verbosity.Default, message);
    }

    public static bool Print(Verbosity verbosity, string message)
    {
        message ??= string.Empty;
        return Print_helper(verbosity, message);
    }

    public static bool PrintError(string message)
    {
        return Print(Verbosity.Always, message);
    }

    public static bool PrintError(string format, params object[] fields)
    {
        return Print(Verbosity.Always, format, fields);
    }

    public static bool PrintNoNewline(string format, params object[] fields)
    {
        return Print_helper(Verbosity.Default, format, true, fields);
    }

    public static bool PrintNoNewline(Verbosity verbosity, string format, params object[] fields)
    {
        return Print_helper(verbosity, format, true, fields);
    }

    public static bool PrintNoNewline(string message = null)
    {
        message ??= string.Empty;
        return Print_helper(Verbosity.Default, message, true);
    }

    public static bool PrintNoNewline(Verbosity verbosity, string message)
    {
        message ??= string.Empty;
        return Print_helper(verbosity, message, true);
    }

    private static bool Print_helper(Verbosity verbosity, string format, bool noNewline, params object[] fields)
    {
        if (CommandLineParserBase.verbosity >= verbosity)
        {
            var message = string.Format(format, fields);
            return Print_helper(verbosity, message, noNewline);
        }

        return false;
    }

    private static bool Print_helper(Verbosity verbosity, string message, bool noNewline = false)
    {
        if (CommandLineParserBase.verbosity >= verbosity)
        {
            if (message.Contains('\n'))
            {
                message = message.Replace("\r\n", "{NL}").Replace("\n", "{NL}").Replace("{NL}", Environment.NewLine);
            }

            if (noNewline)
            {
                System.Console.Write(message);
            }
            else
            {
                System.Console.WriteLine(message);
            }

            return true;
        }

        return false;
    }
}