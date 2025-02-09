using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.SlidingWindows;

public static class WindowingHelper
{
    public static IEnumerable<Window<T>> SplitIntoWindows<T>(IEnumerable<T> items, TimeSpan windowDuration)
        where T : ITimeRanged
    {
        return SplitIntoWindowsCore(items, windowDuration, 0, -1, TimeSpan.Zero);
    }

    public static IEnumerable<Window<T>> SplitIntoWindows<T>(IEnumerable<T> items, TimeSpan windowDuration,
        uint windowsToSkip, uint maxWindowsToTake) where T : ITimeRanged
    {
        return SplitIntoWindowsCore(items, windowDuration, windowsToSkip, (int)maxWindowsToTake, TimeSpan.Zero);
    }

    public static IEnumerable<Window<T>> SplitIntoWindows<T>(IEnumerable<T> items, TimeSpan windowDuration,
        TimeSpan jumpDuration) where T : ITimeRanged
    {
        return SplitIntoWindowsCore(items, windowDuration, 0, -1, jumpDuration);
    }

    private static IEnumerable<Window<T>> SplitIntoWindowsCore<T>(IEnumerable<T> items, TimeSpan windowDuration,
        uint windowsToSkip, int maxWindowsToTake, TimeSpan jumpDuration) where T : ITimeRanged
    {
        var windowsCount = 0;
        using var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            yield break;
        }

        var item = enumerator.Current;
        var from = item.GetFromTimestamp();
        var upto = from + windowDuration;

        var list = new List<T>();
        var reachedEnd = false;
        while (!reachedEnd)
        {
            var itemFrom = item.GetFromTimestamp();
            //var itemUpto = item.GetUptoTimestamp();

            if (itemFrom >= from)
            {
                list.Add(item);
                reachedEnd = !enumerator.MoveNext();
                if (!reachedEnd)
                {
                    item = enumerator.Current;
                }
            }
            else
            {
                windowsCount++;
                if (windowsCount > windowsToSkip)
                {
                    yield return new Window<T>(from, upto, list.AsReadOnly());
                }

                if (maxWindowsToTake > -1)
                {
                    if (windowsCount >= windowsToSkip + maxWindowsToTake)
                    {
                        yield break;
                    }
                }

                list = [];

                upto = from - jumpDuration;
                from = from - windowDuration - jumpDuration;
            }
        }

        if (list.Count > 0)
        {
            yield return new Window<T>(from, upto, list.AsReadOnly());
        }
    }
}