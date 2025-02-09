using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class DynamicEqualityComparer<T> : IEqualityComparer<T>
{
    private static readonly Func<T, T, bool> compareFunc;

    [ILEmitter]
    static DynamicEqualityComparer()
    {
        Type[] parameterTypes = [typeof(T), typeof(T)];
        Label? nextLabel = null;
        var properties = ReflectionHelper.GetPublicProperties<T>(false);
        var dynamicMethod = new DynamicMethod("DynamicEquals", typeof(bool), parameterTypes,
            typeof(DynamicEqualityComparer<T>));
        var ilGen = dynamicMethod.GetILGenerator();

        foreach (var property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            if (nextLabel != null)
            {
                ilGen.MarkLabel(nextLabel.Value);
            }

            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Callvirt, property.GetMethod);

            if (property.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, property.PropertyType);
            }

            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Callvirt, property.GetMethod);

            if (property.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, property.PropertyType);
            }

            ilGen.Emit(OpCodes.Call, GeneralHelper.CompareMethod);

            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Ceq);
            nextLabel = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brtrue, nextLabel.Value);

            ilGen.Emit(OpCodes.Ldc_I4, 0);

            ilGen.Emit(OpCodes.Ret);
        }

        if (nextLabel != null)
        {
            ilGen.MarkLabel(nextLabel.Value);
        }

        ilGen.Emit(OpCodes.Ldc_I4, 1);
        ilGen.Emit(OpCodes.Ret);
        compareFunc = dynamicMethod.CreateDelegate<Func<T, T, bool>>();
    }

    private DynamicEqualityComparer()
    {
    }

    public static DynamicEqualityComparer<T> Instance { get; } = new();

    public bool Equals(T x, T y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if ((x == null && y != null) || (y == null && x != null))
        {
            return false;
        }

        return compareFunc(x, y);
    }

    public int GetHashCode(T obj)
    {
        if (obj == null)
        {
            return 0;
        }

        return DynamicToStringHelper<T>.ExportAsString(obj).GetHashCode();
    }
}