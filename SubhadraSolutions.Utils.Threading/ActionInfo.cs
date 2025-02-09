using System;

namespace SubhadraSolutions.Utils.Threading;

public class ActionInfo(Action action, int maximumRetryCount, int retrySleepTime = 0, string name = null)
{
    public Action Action { get; } = action;
    public int MaximumRetryCount { get; } = Math.Max(maximumRetryCount, 0);
    public string Name { get; } = name;
    public int RetrySleepTime { get; } = Math.Max(0, retrySleepTime);
}