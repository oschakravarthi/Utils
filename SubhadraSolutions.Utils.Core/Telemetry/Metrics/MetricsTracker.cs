using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Exposition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Timers;

namespace SubhadraSolutions.Utils.Telemetry.Metrics;

public sealed class MetricsTracker
{
    private class WeakReferenceInfo(WeakReference weakReference, Metric[] metrics, string className)
    {
        public WeakReference WeakReference { get; } = weakReference;
        public Metric[] Metrics { get; } = metrics;
        public string ClassName { get; } = className;
    }

    private class DelegateInfo(Delegate @delegate, int metricsCount)
    {
        public Delegate Delegate { get; } = @delegate;
        public int MetricsCount { get; } = metricsCount;
    }

    private static readonly ConstructorInfo
        PerformanceCounterConstructor = typeof(Metric).GetConstructors().First();

    private static readonly MethodInfo
        ResetableResetMethod = typeof(IResetable).GetMethod(nameof(IResetable.Reset));

    private readonly ConcurrentDictionary<Type, DelegateInfo> delegates = new();
    private readonly ConcurrentDictionary<string, WeakReferenceInfo> weakReferences = new();
    private List<ObjectMetrics> _capturedMetrics = [];
    private TimeSpan interval = TimeSpan.FromSeconds(5);
    private bool isEnabled;
    private readonly System.Timers.Timer timer;
    private AutoResetEvent resetEvent = new AutoResetEvent(true);

    private MetricsTracker()
    {
        timer = new System.Timers.Timer();
        timer.Enabled = Enabled;
        timer.Interval = interval.TotalMilliseconds;
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    public static MetricsTracker Instance { get; } = new();

    public bool Enabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                timer.Enabled = value;
            }
        }
    }

    public TimeSpan Interval
    {
        get => interval;
        set
        {
            if (interval != value)
            {
                interval = value;
                timer.Interval = value.TotalMilliseconds;
            }
        }
    }

    public MetricsTrackerMetrics Metrics { get; } = new();

    public event Action OnMetricsCaptured;

    [Expose]
    public List<ObjectMetrics> GetMetrics()
    {
        return _capturedMetrics;
    }

    [Expose(httpRequestMethod: HttpRequestMethod.Delete)]
    public void ResetMetrics()
    {
        try
        {
            this.resetEvent.WaitOne();
            ResetMetricsCore();
            this.Metrics.Reset();
            CaptureMetricsCore();
        }
        finally
        {
            this.resetEvent.Set();
        }
    }

    public bool HasMetrics(Type type)
    {
        var members = GetMetricMembers(type);
        return members.Any();
    }

    public bool Register(string name, object obj)
    {
        var members = GetMetricMembers(obj.GetType()).ToList();
        if (members.Count == 0)
        {
            return false;
        }

        var delegateInfo = delegates.GetOrAdd(obj.GetType(), t => BuildDelegate(t, members));
        var weakReference = new WeakReference(obj);
        var metrics = new Metric[delegateInfo.MetricsCount];
        weakReferences.TryAdd(name,
            new WeakReferenceInfo(weakReference, metrics, obj.GetType().Name));
        delegateInfo.Delegate.DynamicInvoke(obj, metrics);

        return true;
    }

    private static IEnumerable<MemberInfo> GetMetricMembers(Type type)
    {
        var members = type.GetProperties().Select(x => (MemberInfo)x)
            .Concat(type.GetFields().Select(x => (MemberInfo)x))
            .Where(m => m.IsDefined(typeof(MetricAttribute), true));
        return members;
    }

    private DelegateInfo BuildDelegate(Type type, IList<MemberInfo> members)
    {
        var performanceCounterArrayType = typeof(Metric[]);
        var dm = new DynamicMethod("dm" + GeneralHelper.Identity, typeof(void),
            [type, performanceCounterArrayType], typeof(MetricsTracker));
        var ilGen = dm.GetILGenerator();
        for (var i = 0; i < members.Count; i++)
        {
            var member = members[i];
            var metricAttribute = member.GetCustomAttribute<MetricAttribute>();
            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldc_I4, i);
            EmitCounterReadingInstruction(member, metricAttribute.MetricType, ilGen);
            ilGen.Emit(OpCodes.Stelem, typeof(Metric));
        }

        //if (typeof(IResetable).IsAssignableFrom(type))
        //{
        //    ilGen.Emit(OpCodes.Ldarg, 0);
        //    ilGen.Emit(OpCodes.Callvirt, ResetableResetMethod);
        //}

        ilGen.Emit(OpCodes.Ret);
        var del = dm.CreateDelegate(typeof(Action<,>).MakeGenericType(type, performanceCounterArrayType));
        return new DelegateInfo(del, members.Count);
    }

    private void EmitCounterReadingInstruction(MemberInfo member, MetricType metricType, ILGenerator ilGen)
    {
        ilGen.Emit(OpCodes.Ldstr, member.Name);

        ilGen.Emit(OpCodes.Ldarg, 0);

        Type returnType;
        if (member.MemberType == MemberTypes.Property)
        {
            var propertyInfo = (PropertyInfo)member;
            returnType = propertyInfo.PropertyType;
            ilGen.Emit(OpCodes.Callvirt, propertyInfo.GetGetMethod());
        }
        else
        {
            var fieldInfo = (FieldInfo)member;
            returnType = fieldInfo.FieldType;
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);
        }

        if (returnType != typeof(double))
        {
            ilGen.Emit(OpCodes.Conv_R8);
        }
        ilGen.Emit(OpCodes.Ldc_I4, (int)metricType);

        ilGen.Emit(OpCodes.Newobj, PerformanceCounterConstructor);
    }

    private void ReadMetrics(object obj, Metric[] metrics)
    {
        var type = obj.GetType();
        delegates.TryGetValue(type, out var delegateInfo);
        delegateInfo.Delegate.DynamicInvoke(obj, metrics);
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        try
        {
            this.resetEvent.WaitOne();
            CaptureMetricsCore();
        }
        finally
        {
            this.resetEvent.Set();
        }
    }

    private void ResetMetricsCore()
    {
        foreach (var kvp in weakReferences)
        {
            var weakReferenceInfo = kvp.Value;

            var reference = weakReferenceInfo.WeakReference.Target;
            if (reference is IResetable resetable)
            {
                resetable.Reset();
            }
        }
    }

    private void CaptureMetricsCore()
    {
        var ticksStart = SharedStopwatch.ElapsedTicks;
        var result = new List<ObjectMetrics>();
        foreach (var kvp in weakReferences)
        {
            var weakReferenceInfo = kvp.Value;
            var metrics = weakReferenceInfo.Metrics;
            var reference = weakReferenceInfo.WeakReference.Target;
            if (reference != null)
            {
                ReadMetrics(reference, metrics);
            }

            result.Add(new ObjectMetrics(weakReferenceInfo.ClassName, kvp.Key, weakReferenceInfo.Metrics, reference == null));
        }

        var ticksTaken = SharedStopwatch.ElapsedTicks - ticksStart;
        Metrics.MetricsCaptured(ticksTaken);
        var met = new Metric[]
        {
            new(nameof(MetricsTrackerMetrics.NumberofTimesMetricsCaptured), Metrics.NumberofTimesMetricsCaptured, MetricType.Snapshot),
            new(nameof(MetricsTrackerMetrics.TicksTaken), Metrics.TicksTaken, MetricType.Snapshot),
            new(nameof(MetricsTrackerMetrics.AverageTicksTakenPerCapture), Metrics.AverageTicksTakenPerCapture, MetricType.Snapshot)
        };
        result.Add(new ObjectMetrics(nameof(MetricsTrackerMetrics), nameof(MetricsTrackerMetrics), met, false));

        _capturedMetrics = result;

        var action = OnMetricsCaptured;
        if (action != null)
        {
            action();
        }
    }
}