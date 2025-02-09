using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils;

[Pure]
public static class DynamicComparerHelper<T>
{
    private static readonly Dictionary<string, IComparer<T>> dictionary = [];

    static DynamicComparerHelper()
    {
        var parameterTypes = new[] { typeof(T), typeof(T) };

        var properties = ReflectionHelper.GetPublicProperties<T>(false);
        foreach (var property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            var dynamicMethod = new DynamicMethod("comparer_" + property.Name, typeof(int), parameterTypes,
                typeof(DynamicComparerHelper<T>));
            var ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Callvirt, property.GetMethod);
            if (property.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, property.PropertyType);
            }

            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Callvirt, property.GetMethod);
            if (property.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, property.PropertyType);
            }

            ilGen.Emit(OpCodes.Call, GeneralHelper.CompareMethod);
            ilGen.Emit(OpCodes.Ret);

            var objectComparison = (Comparison<object>)dynamicMethod.CreateDelegate(typeof(Comparison<object>));
            var comparer = new DelegateBasedComparer<T>((x, y) =>
            {
                if (x != null && y != null)
                {
                    return objectComparison(x, y);
                }

                if (x == null && y == null)
                {
                    return 0;
                }

                return x == null ? 1 : -1;
            });

            dictionary.Add(property.Name, comparer);
        }
    }

    public static IComparer<T> GetComparerForProperty(string propertyName)
    {
        Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(propertyName, propertyName);
        dictionary.TryGetValue(propertyName, out var comparer);
        return comparer;
    }
}