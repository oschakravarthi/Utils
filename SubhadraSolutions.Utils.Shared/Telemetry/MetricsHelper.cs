using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Telemetry;

public static class MetricsHelper
{
    private static readonly MethodInfo BuildListFromMetricsOfSameComponentTypeCoreMethod =
        typeof(MetricsHelper).GetMethod(nameof(BuildListFromMetricsOfSameComponentTypeCore),
            BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo ComponentNameGetterMethod =
        typeof(ObjectMetrics).GetProperty(nameof(ObjectMetrics.ObjectName)).GetGetMethod();

    private static readonly MethodInfo MetricsGetterMethod =
        typeof(ObjectMetrics).GetProperty(nameof(ObjectMetrics.Metrics)).GetGetMethod();

    private static readonly MethodInfo MetricValueGetterMethod =
        typeof(Metric).GetProperty(nameof(Metric.Value)).GetGetMethod();

    private static readonly MethodInfo TimeSpanFromTicksMethod =
        typeof(TimeSpan).GetMethod(nameof(TimeSpan.FromTicks), BindingFlags.Public | BindingFlags.Static);

    private static readonly ConcurrentDictionary<string, Tuple<Type, Delegate>> _lookup = new();

    public static IList BuildListFromMetricsOfSameComponentType(IList<ObjectMetrics> metrics)
    {
        if (metrics == null || metrics.Count == 0)
        {
            return null;
        }

        var tuple = GetMetricsTranspose(metrics[0]);
        var result = (IList)BuildListFromMetricsOfSameComponentTypeCoreMethod.MakeGenericMethod(tuple.Item1)
            .Invoke(null, [metrics, tuple.Item2]);
        return result;
    }

    public static Tuple<Type, Delegate> GetMetricsTranspose(ObjectMetrics metrics)
    {
        var result = _lookup.GetOrAdd(metrics.ObjectType, key => BuildMetricsTranspose(metrics));
        return result;
    }

    private static List<T> BuildListFromMetricsOfSameComponentTypeCore<T>(IEnumerable<ObjectMetrics> metrics,
        Func<ObjectMetrics, T> func)
    {
        var result = new List<T>();
        foreach (var metric in metrics)
        {
            var obj = func(metric);
            result.Add(obj);
        }

        return result;
    }

    private static Tuple<Type, Delegate> BuildMetricsTranspose(ObjectMetrics metrics)
    {
        var tuples = new List<Tuple<string, Type>> { new("Name", typeof(string)) };
        var ms = metrics.Metrics;
        for (var i = 0; i < ms.Length; i++)
        {
            var metric = ms[i];
            var name = metric.Name;
            var ty = typeof(double);
            if (name.Contains("Ticks"))
            {
                name = name.Replace("Ticks", "Time");
                ty = typeof(TimeSpan);
            }

            tuples.Add(new Tuple<string, Type>(name, ty));
        }

        var type = AnonymousTypeBuilder.BuildAnonymousType(tuples);
        var dm = new DynamicMethod("transpose" + GeneralHelper.Identity, type, [typeof(ObjectMetrics)],
            typeof(MetricsHelper));
        var ilGen = dm.GetILGenerator();

        var arrayLocal = ilGen.DeclareLocal(typeof(Metric[]));

        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Callvirt, MetricsGetterMethod);
        ilGen.Emit(OpCodes.Stloc, arrayLocal);

        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Callvirt, ComponentNameGetterMethod);

        for (var i = 0; i < ms.Length; i++)
        {
            ilGen.Emit(OpCodes.Ldloc, arrayLocal);
            ilGen.Emit(OpCodes.Ldc_I4, i);
            ilGen.Emit(OpCodes.Ldelema, typeof(Metric));
            ilGen.Emit(OpCodes.Call, MetricValueGetterMethod);
            if (tuples[i + 1].Item2 == typeof(TimeSpan))
            {
                ilGen.Emit(OpCodes.Conv_I8);
                ilGen.Emit(OpCodes.Call, TimeSpanFromTicksMethod);
            }
        }

        var constructor = type.GetConstructors().First();

        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Ret);
        var del = dm.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(ObjectMetrics), type));

        return new Tuple<Type, Delegate>(type, del);
    }
}