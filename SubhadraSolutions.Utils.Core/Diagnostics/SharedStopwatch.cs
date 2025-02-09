using System;
using System.Diagnostics;

namespace SubhadraSolutions.Utils.Diagnostics;

public static class SharedStopwatch
{
    private static readonly Stopwatch _sw = Stopwatch.StartNew();

    public static TimeSpan Elapsed => _sw.Elapsed;

    public static long ElapsedTicks => _sw.ElapsedTicks;

    public static TimeSpan GetElapsed(long ticksBefore)
    {
        return TimeSpan.FromTicks(_sw.ElapsedTicks - ticksBefore);
    }

    public static TimeSpan PerformActionAndGetTimeTaken(Action action)
    {
        var ticksBefore = ElapsedTicks;
        action();
        return TimeSpan.FromTicks(_sw.ElapsedTicks - ticksBefore);
    }
}