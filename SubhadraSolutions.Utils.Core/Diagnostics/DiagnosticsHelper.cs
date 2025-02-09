using System;
using System.Diagnostics;
using System.Text;

namespace SubhadraSolutions.Utils.Diagnostics;

public static class DiagnosticsHelper
{
    public static string GetStackTrace()
    {
        var sb = new StringBuilder();
        var stackTrace = new StackTrace();
        for (var i = 1; i < stackTrace.FrameCount; i++)
        {
            var stackFrame = stackTrace.GetFrame(i);
            sb.AppendLine(stackFrame.ToString());
        }

        return sb.ToString();
    }

    public static void PrintStackTrace()
    {
        var stackTrace = GetStackTrace();
        Console.WriteLine(stackTrace);
    }
}