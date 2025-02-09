using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

public static class TransposeHelper
{
    [DynamicallyInvoked]
    public static IList Transpose<T>(IReadOnlyList<T> list, string keyPropertyName, string valuePropertyName)
    {
        var elementType = typeof(T);
        var properties = elementType.GetProperties();
        var axisProperty = elementType.GetProperty(keyPropertyName);
        var rotateProperty = elementType.GetProperty(valuePropertyName);

        var func = PropertiesToStringValuesHelper.BuildFuncForToString<T>(new List<string> { keyPropertyName });
        var nonKeyColumns = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);

        for (var i = 0; i < list.Count; i++)
        {
            var value = func(list[i]);
            value = CleanPropertyName(value);
            nonKeyColumns.Add(value);
        }

        var tuples = new List<Tuple<string, Type>>();

        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];

            if (property == axisProperty || property == rotateProperty)
            {
                continue;
            }

            tuples.Add(new Tuple<string, Type>(property.Name, property.PropertyType));
        }

        Type anonymousKeyType = null;

        if (tuples.Count > 0)
        {
            anonymousKeyType = AnonymousTypeBuilder.BuildAnonymousType(tuples);
        }

        var columnIndexes = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var v in nonKeyColumns)
        {
            tuples.Add(new Tuple<string, Type>(v, rotateProperty.PropertyType));
            columnIndexes.Add(v, columnIndexes.Count);
        }

        var anonymousType = AnonymousTypeBuilder.BuildAnonymousType(tuples);

        if (anonymousKeyType != null)
        {
            var method = typeof(TransposeHelper)
                .GetMethod(nameof(TransposeCoreWithKey), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(typeof(T), anonymousKeyType, anonymousType, rotateProperty.PropertyType);

            return (IList)method.Invoke(null, [list, valuePropertyName, func, columnIndexes]);
        }
        else
        {
            var method = typeof(TransposeHelper)
                .GetMethod(nameof(TransposeCoreWithoutKey), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(typeof(T), anonymousType, rotateProperty.PropertyType);

            return (IList)method.Invoke(null, [list, valuePropertyName, func, columnIndexes]);
        }
    }

    public static IList TransposeList(object list, string keyPropertyName, string valuePropertyName)
    {
        var elementType = list.GetType().GetEnumerableItemType();

        var method = typeof(TransposeHelper).GetMethod(nameof(Transpose), BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(elementType);

        return (IList)method.Invoke(null, [list, keyPropertyName, valuePropertyName]);
    }

    [ILEmitter]
    private static Func<TFrom, TTo> BuildKeyFunc<TFrom, TTo>()
    {
        var fromType = typeof(TFrom);
        var toType = typeof(TTo);

        var dm = new DynamicMethod("createKeyDynamicMethod" + GeneralHelper.Identity, toType, [fromType],
            typeof(TransposeHelper));
        var ilGen = dm.GetILGenerator();
        var keyProperties = toType.GetProperties();

        for (var i = 0; i < keyProperties.Length; i++)
        {
            var sourceProperty = fromType.GetProperty(keyProperties[i].Name);
            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Callvirt, sourceProperty.GetMethod);
        }

        var constructor = toType.GetConstructors()[0];
        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<TFrom, TTo>>();
    }

    [ILEmitter]
    private static Func<TValue[], TTo> BuildTargetFunc<TTo, TValue>(int arrayItemsCount)
    {
        var toType = typeof(TTo);

        var dm = new DynamicMethod("createTargetDynamicMethod" + GeneralHelper.Identity, toType,
            [typeof(TValue[])], typeof(TransposeHelper));
        var ilGen = dm.GetILGenerator();

        for (var i = 0; i < arrayItemsCount; i++)
        {
            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldc_I4, i);
            ilGen.Emit(OpCodes.Ldelem);
        }

        var constructor = toType.GetConstructors()[0];
        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<TValue[], TTo>>();
    }

    [ILEmitter]
    private static Func<TFrom, TValue[], TTo> BuildTargetFunc<TFrom, TTo, TValue>(int arrayItemsCount)
    {
        var fromType = typeof(TFrom);
        var toType = typeof(TTo);

        var dm = new DynamicMethod("createTargetDynamicMethod" + GeneralHelper.Identity, toType,
            [fromType, typeof(TValue[])], typeof(TransposeHelper));
        var ilGen = dm.GetILGenerator();
        //var local = ilGen.DeclareLocal(typeof(TValue));
        var keyProperties = fromType.GetProperties();

        for (var i = 0; i < keyProperties.Length; i++)
        {
            var sourceProperty = fromType.GetProperty(keyProperties[i].Name);
            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Callvirt, sourceProperty.GetMethod);
        }

        for (var i = 0; i < arrayItemsCount; i++)
        {
            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldc_I4, i);
            ilGen.Emit(OpCodes.Ldelem, typeof(TValue));
            //ilGen.Emit(OpCodes.Stloc, 0);
            //ilGen.Emit(OpCodes.Ldloc, 0);
        }

        var constructor = toType.GetConstructors()[0];
        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<TFrom, TValue[], TTo>>();
    }

    [ILEmitter]
    private static Func<TFrom, TTo> BuildValueFunc<TFrom, TTo>(string valuePropertyName)
    {
        var fromType = typeof(TFrom);
        var toType = typeof(TTo);

        var dm = new DynamicMethod("createKeyDynamicMethod" + GeneralHelper.Identity, toType, [fromType],
            typeof(TransposeHelper));
        var ilGen = dm.GetILGenerator();
        var property = fromType.GetProperty(valuePropertyName);
        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Callvirt, property.GetMethod);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<TFrom, TTo>>();
    }

    private static string CleanPropertyName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            value = "NULL";
        }

        //value = value.Replace(' ', '_').Replace('.', '_');
        //if (char.IsDigit(value[0]))
        //{
        //    value = '_' + value;
        //}
        return value;
    }

    [DynamicallyInvoked]
    private static List<TTarget> TransposeCoreWithKey<TSource, TKey, TTarget, TValue>(IEnumerable<TSource> enumerable,
        string rotatePropertyName, Func<TSource, string> axisColumnValueFunc,
        Dictionary<string, int> nonKeyColumnsAndIndexes)
    {
        var dictionary = new Dictionary<TKey, TValue[]>(DynamicEqualityComparer<TKey>.Instance);
        var sourceToKeyFunc = BuildKeyFunc<TSource, TKey>();
        var valueFunc = BuildValueFunc<TSource, TValue>(rotatePropertyName);
        var keyToTargetFunc = BuildTargetFunc<TKey, TTarget, TValue>(nonKeyColumnsAndIndexes.Count);

        foreach (var item in enumerable)
        {
            var key = sourceToKeyFunc(item);

            if (!dictionary.TryGetValue(key, out var array))
            {
                array = new TValue[nonKeyColumnsAndIndexes.Count];
                dictionary.Add(key, array);
            }

            var axisColumnValue = axisColumnValueFunc(item);
            axisColumnValue = CleanPropertyName(axisColumnValue);
            var value = valueFunc(item);
            var index = nonKeyColumnsAndIndexes[axisColumnValue];
            array[index] = value;
        }

        var result = new List<TTarget>(dictionary.Count);

        foreach (var kvp in dictionary)
        {
            var target = keyToTargetFunc(kvp.Key, kvp.Value);
            result.Add(target);
        }

        return result;
    }

    [DynamicallyInvoked]
    private static List<TTarget> TransposeCoreWithoutKey<TSource, TTarget, TValue>(IEnumerable<TSource> enumerable,
        string valuePropertyName, Func<TSource, string> axisColumnValueFunc,
        Dictionary<string, int> nonKeyColumnsAndIndexes)
    {
        var array = new TValue[nonKeyColumnsAndIndexes.Count];

        var valueFunc = BuildValueFunc<TSource, TValue>(valuePropertyName);
        var keyToTargetFunc = BuildTargetFunc<TTarget, TValue>(nonKeyColumnsAndIndexes.Count);

        foreach (var item in enumerable)
        {
            var axisColumnValue = axisColumnValueFunc(item);
            axisColumnValue = CleanPropertyName(axisColumnValue);
            var index = nonKeyColumnsAndIndexes[axisColumnValue];
            array[index] = valueFunc(item);
        }

        var result = new List<TTarget>(1);
        var target = keyToTargetFunc(array);
        result.Add(target);
        return result;
    }
}