using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Diagnostics.Tracing;
using System;
using System.Threading;

namespace SubhadraSolutions.Utils;

public static class DurationHelper
{
    private static int _durationActionIndex;

    public static int GetActionIndex()
    {
        return Interlocked.Increment(ref _durationActionIndex);
    }

    public static TimeSpan GetDuration(Action action, bool logAsDebug = false, string prefix = null,
        string suffix = null, string strAction = null, bool reportDuration = true, int? actionIndex = null)
    {
        var num = actionIndex ?? GetActionIndex();
        var ticksStart = SharedStopwatch.ElapsedTicks;
        action();
        var timeSpan = SharedStopwatch.GetElapsed(ticksStart);
        if (reportDuration)
        {
            ReportDuration(timeSpan, prefix, suffix, strAction, logAsDebug, "Utils.GetDuration(Action, ...)", num);
        }

        return timeSpan;
    }

    public static TimeSpan GetDuration<TReturn>(Func<TReturn> func, out TReturn result, bool logAsDebug = false,
        string prefix = null, string suffix = null, string strAction = null, bool reportDuration = true,
        int? actionIndex = null)
    {
        result = default;
        var num = actionIndex ?? GetActionIndex();
        var ticksStart = SharedStopwatch.ElapsedTicks;
        result = func();
        var timeSpan = SharedStopwatch.GetElapsed(ticksStart);
        if (reportDuration)
        {
            ReportDuration(timeSpan, prefix, suffix, strAction, logAsDebug,
                "Utils.GetDuration(Func<" + typeof(TReturn).Name + ">, ...)", num);
        }

        return timeSpan;
    }

    public static void ReportDuration(TimeSpan duration, string prefix = null, string suffix = null,
        string strAction = null, bool logAsDebug = false, string nameFn = null, int actionIndex = 0)
    {
        ReportDuration((int)duration.TotalMilliseconds, prefix, suffix, strAction, logAsDebug, nameFn, actionIndex);
    }

    public static void ReportDuration(int duration, string prefix = null, string suffix = null,
        string strAction = null, bool logAsDebug = false, string nameFn = null, int actionIndex = 0)
    {
        if (string.IsNullOrEmpty(nameFn))
        {
            nameFn = ActiveCode.Parent.Name;
        }

        var text = "-T- " + nameFn + ": ";
        if (strAction != null)
        {
            if (prefix == null)
            {
                prefix = strAction.Contains("Wait") || strAction.Contains("Sleep") ? "Waited: " : "Runtime:";
            }

            if (suffix == null)
            {
                suffix = ".  Action: " + strAction;
            }
        }
        else
        {
            suffix = suffix != null ? " " + suffix : ".";
        }

        text +=
            $"|{TimeSpanHelper.DescribeTimeSpan(duration)}|{(actionIndex > 0 ? "#" + actionIndex + ": " : "")}{(prefix != null ? " " + prefix : "")}{suffix}";
        if (!logAsDebug)
        {
            TraceLogger.TraceInformation(text);
        }
    }
}