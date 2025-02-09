using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;

namespace SubhadraSolutions.Utils;

public static class DynamicToStringHelper<T>
{
    private static readonly Func<T, string> func = PropertiesToStringValuesHelper.BuildFuncForToString<T>();

    [DynamicallyInvoked]
    public static string ExportAsString(T obj)
    {
        return func(obj);
    }
}