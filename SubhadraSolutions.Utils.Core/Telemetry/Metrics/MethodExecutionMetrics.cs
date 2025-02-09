using SubhadraSolutions.Utils.Contracts;
using System.Reflection;
using System.Threading;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public class MethodExecutionMetrics : INamed
{
    private long _numberOfTimesTheMethodIsExecuted;

    public MethodExecutionMetrics(MethodInfo method)
    {
        Method = method;
        Name = GetMethodUniqueName(method);
        MetricsTracker.Instance.Register(Name, this);
    }

    public MethodInfo Method { get; }

    [Metric(MetricType.Snapshot)]
    public long NumberOfTimesTheMethodIsExecuted => Interlocked.Read(ref _numberOfTimesTheMethodIsExecuted);

    public string Name { get; }

    public static string GetMethodUniqueName(MethodInfo method)
    {
        return $"{method.DeclaringType.FullName}.{method.Name}";
    }

    public void MethodExecuted(long ticksTaken)
    {
        Interlocked.Increment(ref _numberOfTimesTheMethodIsExecuted);
    }
}