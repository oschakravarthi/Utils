using Microsoft.Extensions.Logging;
using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Reflection.Helpers;
using SubhadraSolutions.Utils.Telemetry;
using SubhadraSolutions.Utils.Telemetry.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Instrumentation.Instruments;

public sealed class MethodInstrument
{
    private static readonly MethodInfo DictionaryAddMethod = DictionaryType.GetMethod("Add");

    private static readonly ConstructorInfo DictionaryDefaultConstructor =
        DictionaryType.GetConstructor(Type.EmptyTypes);

    private static readonly Type DictionaryType = typeof(Dictionary<string, object>);

    private static readonly MethodInfo GetIdentityGetter = typeof(GeneralHelper)
        .GetProperty(nameof(GeneralHelper.Identity), BindingFlags.Public | BindingFlags.Static).GetGetMethod();

    private static readonly MethodInfo GetMethodExecutionMetricsMethod =
        typeof(MethodInstrument).GetMethod(nameof(GetMethodExecutionMetrics),
            BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo LoggerGetter = typeof(MethodInstrument)
        .GetProperty(nameof(Logger), BindingFlags.Public | BindingFlags.Static).GetGetMethod();

    private static readonly MethodInfo LogMethodExecutionMethod =
        typeof(MethodInstrument).GetMethod(nameof(LogMethodExecution), BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly ConstructorInfo MethodExecutionLogItemConstructor =
        typeof(MethodExecutionLogItem).GetConstructors().First();

    //private static Dictionary<MethodInfo, MethodExecutionMetrics> methodExecutionMetricsLookup=new Dictionary<MethodInfo, MethodExecutionMetrics>();
    private static readonly Dictionary<string, MethodExecutionMetrics> methodExecutionMetricsLookup = [];

    private static readonly MethodInfo MethodExecutionMetricsMethodExecutedMethod =
        typeof(MethodExecutionMetrics).GetMethod(nameof(MethodExecutionMetrics.MethodExecuted),
            BindingFlags.Public | BindingFlags.Instance);

    //private static FieldInfo MethodExecutionMetricsLookupField = typeof(MethodInstrument).GetField(nameof(methodExecutionMetricsLookup), BindingFlags.NonPublic | BindingFlags.Static);
    private static readonly MethodInfo SharedStopwatchElapsedTicksGetter = typeof(SharedStopwatch)
        .GetProperty(nameof(SharedStopwatch.ElapsedTicks), BindingFlags.Public | BindingFlags.Static).GetGetMethod();

    static MethodInstrument()
    {
        ConfigSelector = GetMethodInstrumentationConfig;
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
    }

    public static Func<MethodInfo, MethodInstrumentationConfig> ConfigSelector { get; set; }

    public static ILogger Logger { get; set; }

    private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        foreach (var type in args.LoadedAssembly.GetTypes())
        {
            foreach (var method in type.GetMethods())
            {
                if (!method.IsAbstract)
                {
                    var config = ConfigSelector(method);
                    if (config == null)
                    {
                        continue;
                    }

                    var dm = InstrumentMethod(method, config);
                    if (dm != null)
                    {
                        MethodHelper.ReplaceMethod(method, dm);
                    }
                }
            }
        }
    }

    private static MethodExecutionMetrics GetMethodExecutionMetrics(string key)
    {
        return methodExecutionMetricsLookup[key];
    }

    private static MethodInstrumentationConfig GetMethodInstrumentationConfig(MethodInfo method)
    {
        var metricsAttribute = method.GetCustomAttribute<InstrumentMetricsAttribute>(true);
        var logAttribute = method.GetCustomAttribute<InstrumentLogAttribute>(true);
        var instrumentMetrics = metricsAttribute != null;
        var logOption = logAttribute == null ? MethodLogOption.None : logAttribute.LogOption;
        var config = new MethodInstrumentationConfig(instrumentMetrics, logOption);
        return config;
    }

    private static DynamicMethod InstrumentMethod(MethodInfo method, MethodInstrumentationConfig config)
    {
        if (!config.InstrumentMetrics && config.LogOption == MethodLogOption.None)
        {
            return null;
        }

        var type = method.DeclaringType;
        var key = MethodExecutionMetrics.GetMethodUniqueName(method);
        if (config.InstrumentMetrics)
        {
            var metrics = new MethodExecutionMetrics(method);
            methodExecutionMetricsLookup.Add(key, metrics);
        }

        var actualParameters = method.GetParameters();
        var actualParameterTypes = actualParameters.Select(x => x.ParameterType).ToArray();
        var parameterTypes = new List<Type>();
        if (!method.IsStatic)
        {
            parameterTypes.Add(type);
        }

        parameterTypes.AddRange(actualParameterTypes);

        var dm = new DynamicMethod(method.Name + GeneralHelper.Identity, method.ReturnType, parameterTypes.ToArray(),
            type);
        var ilGen = dm.GetILGenerator();

        LocalBuilder returnLocal = null;
        if (method.ReturnType != typeof(void))
        {
            returnLocal = ilGen.DeclareLocal(method.ReturnType);
        }

        var runIdentity = ilGen.DeclareLocal(typeof(long));
        var ticksStart = ilGen.DeclareLocal(typeof(long));
        var ticksEnd = ilGen.DeclareLocal(typeof(long));
        var ticksTaken = ilGen.DeclareLocal(typeof(long));
        var exceptionLocal = ilGen.DeclareLocal(typeof(Exception));
        var dictionaryLocal = ilGen.DeclareLocal(DictionaryType);
        var logItem = ilGen.DeclareLocal(typeof(MethodExecutionLogItem));

        EmitHelper.InitializeLocal(exceptionLocal, ilGen);
        EmitHelper.InitializeLocal(dictionaryLocal, ilGen);
        EmitHelper.InitializeLocal(logItem, ilGen);

        ilGen.Emit(OpCodes.Call, GetIdentityGetter);
        ilGen.Emit(OpCodes.Stloc, runIdentity);

        ilGen.Emit(OpCodes.Call, SharedStopwatchElapsedTicksGetter);
        ilGen.Emit(OpCodes.Stloc, ticksStart);
        var exBlock = ilGen.BeginExceptionBlock();

        for (var i = 0; i < parameterTypes.Count; i++) ilGen.Emit(OpCodes.Ldarg, i);
        ilGen.Emit(method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, method);

        if (returnLocal != null)
        {
            ilGen.Emit(OpCodes.Stloc, returnLocal);
        }

        ilGen.Emit(OpCodes.Leave_S, exBlock);

        if (config.LogOption != MethodLogOption.None)
        {
            ilGen.BeginCatchBlock(typeof(Exception));
            ilGen.Emit(OpCodes.Stloc, exceptionLocal);

            ilGen.Emit(OpCodes.Newobj, DictionaryDefaultConstructor);
            ilGen.Emit(OpCodes.Stloc, dictionaryLocal);

            var implicitParametersCount = method.IsStatic ? 0 : 1;
            for (var i = 0; i < actualParameters.Length; i++)
            {
                var parameterName = actualParameters[i].Name;
                ilGen.Emit(OpCodes.Ldloc, dictionaryLocal);
                ilGen.Emit(OpCodes.Ldstr, parameterName);
                ilGen.Emit(OpCodes.Ldarg, i + implicitParametersCount);
                if (parameterTypes[i].IsValueType)
                {
                    ilGen.Emit(OpCodes.Box, parameterTypes[i]);
                }

                ilGen.Emit(OpCodes.Callvirt, DictionaryAddMethod);
            }

            ilGen.Emit(OpCodes.Rethrow);
        }

        ilGen.BeginFinallyBlock();

        ilGen.Emit(OpCodes.Call, SharedStopwatchElapsedTicksGetter);
        ilGen.Emit(OpCodes.Stloc, ticksEnd);

        ilGen.Emit(OpCodes.Ldloc, ticksEnd);
        ilGen.Emit(OpCodes.Ldloc, ticksStart);
        ilGen.Emit(OpCodes.Sub);
        ilGen.Emit(OpCodes.Stloc, ticksTaken);

        if (config.InstrumentMetrics)
        {
            ilGen.Emit(OpCodes.Ldstr, key);
            ilGen.Emit(OpCodes.Call, GetMethodExecutionMetricsMethod);
            ilGen.Emit(OpCodes.Ldloc, ticksTaken);
            ilGen.Emit(OpCodes.Callvirt, MethodExecutionMetricsMethodExecutedMethod);
        }

        if (config.LogOption != MethodLogOption.None)
        {
            ilGen.Emit(OpCodes.Ldstr, key);
            ilGen.Emit(OpCodes.Ldloc, runIdentity);
            ilGen.Emit(OpCodes.Ldloc, ticksTaken);
            ilGen.Emit(OpCodes.Ldloc, exceptionLocal);

            ilGen.Emit(OpCodes.Ldloc, dictionaryLocal);
            ilGen.Emit(OpCodes.Newobj, MethodExecutionLogItemConstructor);
            ilGen.Emit(OpCodes.Stloc, logItem);

            ilGen.Emit(OpCodes.Ldloc, logItem);
            ilGen.Emit(OpCodes.Call, LogMethodExecutionMethod);
        }

        ilGen.EndExceptionBlock();

        if (returnLocal != null)
        {
            ilGen.Emit(OpCodes.Ldloc, returnLocal);
        }

        ilGen.Emit(OpCodes.Ret);

        return dm;
    }

    private static void LogMethodExecution(MethodExecutionLogItem logItem)
    {
        Logger.Log(logItem.Exception == null ? LogLevel.Information : LogLevel.Error, new EventId(), logItem,
            logItem.Exception, null);
    }

    public class MethodExecutionLogItem(string methodName, long runIdentity, long ticksTaken, Exception exception,
        Dictionary<string, object> arguments)
    {
        public Dictionary<string, object> Arguments { get; } = arguments;
        public Exception Exception { get; } = exception;
        public string MethodName { get; } = methodName;
        public long RunIdentity { get; } = runIdentity;
        public long TicksTaken { get; } = ticksTaken;
    }
}