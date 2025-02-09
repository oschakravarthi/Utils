using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SubhadraSolutions.Utils.Reflection;

public static class PropertiesToStringValuesHelper
{
    private static readonly MethodInfo AppendPropertyValueToStringBuilderMethod =
        typeof(PropertiesToStringValuesHelper).GetMethod(nameof(AppendPropertyValueToStringBuilder),
            BindingFlags.Static | BindingFlags.NonPublic);

    private static readonly MethodInfo BuildFuncForToStringAsObjectTemplateMethod =
        typeof(PropertiesToStringValuesHelper).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(BuildFuncForToStringAsObject) && m.IsGenericMethod);

    private static readonly MethodInfo BuildFuncForToStringTemplateMethod = typeof(PropertiesToStringValuesHelper)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == nameof(BuildFuncForToString) && m.IsGenericMethod);

    private static readonly MethodInfo ConstExpressionCreateMethod =
        typeof(Expression).GetMethod(nameof(Expression.Constant), BindingFlags.Static | BindingFlags.Public, null,
            [typeof(object)], null);

    private static readonly char separator = '\n';

    private static readonly MethodInfo toMappedEnumerableTemplateMethod = typeof(CollectionHelper)
        .GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.Name == nameof(CollectionHelper.ToMappedEnumerable) && m.GetParameters().Length == 1);

    static PropertiesToStringValuesHelper()
    {
        CapBeginChar = '[';
        CapEndChar = ']';
    }

    public static char CapBeginChar { get; set; }
    public static char CapEndChar { get; set; }

    [ILEmitter]
    public static Action<T, string[]> BuildActionForToStringArray<T>(IEnumerable<PropertyInfo> properties)
    {
        var objectType = typeof(T);
        var dynamicMethod = new DynamicMethod("DynamicString" + GeneralHelper.Identity, typeof(void),
            [objectType, typeof(string[])], typeof(PropertiesToStringValuesHelper));
        var ilGen = dynamicMethod.GetILGenerator();
        GenerateForBuildActionForToStringArray(properties, ilGen);
        return (Action<T, string[]>)dynamicMethod.CreateDelegate(typeof(Action<T, string[]>));
    }

    [ILEmitter]
    public static Func<T, string> BuildDynamicMethodForToString<T>(PropertyInfo property)
    {
        var objectType = typeof(T);
        var dynamicMethod = new DynamicMethod("DynamicString" + GeneralHelper.Identity, typeof(string),
            [objectType], typeof(PropertiesToStringValuesHelper));
        var ilGen = dynamicMethod.GetILGenerator();

        EmitCommonILForPropertyValueToString(property, ilGen);

        ilGen.Emit(OpCodes.Ret);
        return (Func<T, string>)dynamicMethod.CreateDelegate(typeof(Func<T, string>));
    }

    [ILEmitter]
    public static Func<T, KeyValuePair<string, TValue>> BuildFuncForKeyValuePair<T, TValue>(
        string measurePropertyName, params string[] dimensionPropertyNames)
    {
        var type = typeof(T);
        var returnType = typeof(KeyValuePair<string, TValue>);
        var measureProperty = type.GetProperty(measurePropertyName);
        var dimensionProperties = type.GetProperties()
            .Where(p => p != measureProperty && p.CanRead && dimensionPropertyNames.Contains(p.Name)).ToArray();

        var dm = new DynamicMethod("BuildFuncForKeyValuePair" + GeneralHelper.Identity, returnType, [type],
            typeof(PropertiesToStringValuesHelper));
        var ilGen = dm.GetILGenerator();
        if (dimensionProperties.Length > 1)
        {
            ilGen.DeclareLocal(typeof(StringBuilder));

            var sbConstructor = typeof(StringBuilder).GetConstructor(Type.EmptyTypes);
            ilGen.Emit(OpCodes.Newobj, sbConstructor);
            ilGen.Emit(OpCodes.Stloc, 0);
        }

        if (dimensionProperties.Length > 1)
        {
            for (var i = 0; i < dimensionProperties.Length; i++)
            {
                EmitCommonILForPropertyValueToString(dimensionProperties[i], ilGen);

                ilGen.Emit(OpCodes.Ldloc, 0);
                ilGen.Emit(OpCodes.Call, AppendPropertyValueToStringBuilderMethod);
            }

            ilGen.Emit(OpCodes.Ldloc, 0);
            ilGen.Emit(OpCodes.Call, StringHelper.ObjectToStringMethod);
        }
        else
        {
            EmitCommonILForPropertyValueToString(dimensionProperties[0], ilGen);
        }

        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Callvirt, measureProperty.GetMethod);
        if (measureProperty.PropertyType != typeof(TValue))
        {
            var convertMethod = ConvertHelper.GetConvertMethod(measureProperty.PropertyType, typeof(TValue), true);
            ilGen.Emit(convertMethod.IsStatic ? OpCodes.Call : OpCodes.Callvirt, convertMethod);
        }

        var constructor = returnType.GetConstructors().First();
        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Ret);

        return (Func<T, KeyValuePair<string, TValue>>)dm.CreateDelegate(
            typeof(Func<T, KeyValuePair<string, TValue>>));
    }

    public static object BuildFuncForToString(Type objectType, IReadOnlyList<string> properties)
    {
        return BuildFuncForToStringTemplateMethod.MakeGenericMethod(objectType)
            .Invoke(null, [properties]);
    }

    [DynamicallyInvoked]
    public static Func<T, string> BuildFuncForToString<T>()
    {
        var properties = typeof(T).GetProperties();
        return BuildFuncForToString<T>(properties);
    }

    public static Func<T, string> BuildFuncForToString<T>(IReadOnlyList<string> properties)
    {
        if (properties == null || properties.Count == 0)
        {
            return obj => string.Empty;
        }

        var props = properties.Select(p => typeof(T).GetProperty(p)).ToArray();

        return BuildFuncForToString<T>(props);
    }

    public static Func<T, string> BuildFuncForToString<T>(PropertyInfo[] properties)
    {
        var propertiesCount = properties.Length;
        if (propertiesCount == 1)
        {
            return BuildDynamicMethodForToString<T>(properties[0]);
        }

        var dmForMultipleDimensions = BuildDynamicMethodForToString<T>(properties);

        return obj =>
        {
            if (obj == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            dmForMultipleDimensions(obj, sb);
            return sb.ToString();
        };
    }

    public static Func<T, string[]> BuildFuncForToStringArray<T>()
    {
        var properties = typeof(T).GetProperties();
        return BuildFuncForToStringArray<T>(properties);
    }

    public static Func<T, string[]> BuildFuncForToStringArray<T>(IEnumerable<string> properties)
    {
        var props = properties.Select(p => typeof(T).GetProperty(p)).ToArray();
        return BuildFuncForToStringArray<T>(props);
    }

    public static Func<T, string[]> BuildFuncForToStringArray<T>(PropertyInfo[] properties)
    {
        var propertiesCount = properties.Length;
        var func = BuildActionForToStringArray<T>(properties);

        return obj =>
        {
            var array = new string[propertiesCount];
            func(obj, array);
            return array;
        };
    }

    public static object BuildFuncForToStringAsObject(Type objectType, IReadOnlyList<string> properties)
    {
        return BuildFuncForToStringAsObjectTemplateMethod.MakeGenericMethod(objectType)
            .Invoke(null, [properties]);
    }

    [DynamicallyInvoked]
    public static Func<T, object> BuildFuncForToStringAsObject<T>(IReadOnlyList<string> properties)
    {
        var func = BuildFuncForToString<T>(properties);

        return obj => func(obj);
    }

    public static Func<T, IEnumerable<string>> BuildGetValueAsStringEnumerableFunc<T>(PropertyInfo property)
    {
        var del = BuildGetValueAsStringEnumerableFunc(property);
        return (Func<T, IEnumerable<string>>)del;
    }

    [ILEmitter]
    public static Delegate BuildGetValueAsStringEnumerableFunc(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var dm = new DynamicMethod("getValueAsStringListDynamicMethod" + GeneralHelper.Identity,
            property.PropertyType, [property.DeclaringType], typeof(PropertiesToStringValuesHelper));
        var ilGen = dm.GetILGenerator();
        property.EmitIlForPropertyAccessor(ilGen);

        if (!typeof(IEnumerable<string>).IsAssignableFrom(propertyType))
        {
            ilGen.Emit(OpCodes.Call, toMappedEnumerableTemplateMethod);
        }

        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate(typeof(Func<,>).MakeGenericType(property.DeclaringType,
            typeof(IEnumerable<string>)));
    }

    public static Func<T, TPropertyType> BuildGetValueAsStringListFunc<T, TPropertyType>(PropertyInfo property)
    {
        return (Func<T, TPropertyType>)property.BuildPropertyAccessor();
    }

    public static Expression[] BuildValueExpressionFromStringValues(Type objectType,
        IReadOnlyList<KeyValuePair<string, string>> propertyNamesAndValues)
    {
        PropertyInfo[] props = null;

        if (propertyNamesAndValues == null || propertyNamesAndValues.Count == 0)
        {
            props = objectType.GetProperties();
        }
        else
        {
            props = propertyNamesAndValues.Select(kvp => kvp.Key).Select(objectType.GetProperty).ToArray();
        }

        if (props.Length == 0)
        {
            return Array.Empty<Expression>();
        }

        var values = propertyNamesAndValues.Select(kvp => kvp.Value).ToArray();
        var expressions = new Expression[propertyNamesAndValues.Count];
        var func = BuildDynamicMethodForWhereClause(props);
        func(values, expressions);
        return expressions;
    }

    public static string[] GetWhereArguments(string value)
    {
        if (value == null)
        {
            return [null];
        }

        if (value?.Length == 0)
        {
            return [string.Empty];
        }

        return value.Replace(CapEndChar + CapBeginChar.ToString(), separator.ToString())
            .Trim(CapBeginChar, CapEndChar).Split(separator);
    }

    [DynamicallyInvoked]
    private static void AppendPropertyValueToStringBuilder(string propertyValue, StringBuilder sb)
    {
        sb.Append(CapBeginChar).Append(propertyValue).Append(CapEndChar);
    }

    [ILEmitter]
    private static Action<T, StringBuilder> BuildDynamicMethodForToString<T>(PropertyInfo[] properties)
    {
        var objectType = typeof(T);
        var dynamicMethod = new DynamicMethod("DynamicString" + GeneralHelper.Identity, typeof(void),
            [objectType, typeof(StringBuilder)], typeof(PropertiesToStringValuesHelper));
        var ilGen = dynamicMethod.GetILGenerator();

        for (var i = 0; i < properties.Length; i++)
        {
            EmitCommonILForPropertyValueToString(properties[i], ilGen);

            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Call, AppendPropertyValueToStringBuilderMethod);
        }

        ilGen.Emit(OpCodes.Ret);
        return (Action<T, StringBuilder>)dynamicMethod.CreateDelegate(typeof(Action<T, StringBuilder>));
    }

    [ILEmitter]
    private static Action<string[], Expression[]> BuildDynamicMethodForWhereClause(PropertyInfo[] properties)
    {
        var dynamicMethod = new DynamicMethod("DynamicWhere" + GeneralHelper.Identity, typeof(void),
            [typeof(string[]), typeof(Expression[])], typeof(PropertiesToStringValuesHelper));
        var ilGen = dynamicMethod.GetILGenerator();
        ilGen.DeclareLocal(typeof(ConstantExpression));

        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            var propertyType = property.PropertyType;

            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Ldc_I4, i);
            ilGen.Emit(OpCodes.Ldelem_Ref);

            if (propertyType.IsEnum)
            {
                ilGen.Emit(OpCodes.Ldobj, propertyType);
            }

            if (propertyType != typeof(string))
            {
                if (propertyType.IsEnum)
                {
                    ilGen.Emit(OpCodes.Call, SharedReflectionInfo.EnumParseMethod);
                    ilGen.Emit(OpCodes.Unbox, propertyType);
                }
                else
                {
                    if (propertyType.IsValueType)
                    {
                        if (propertyType.IsDateOrTimeType())
                        {
                            ilGen.Emit(OpCodes.Call, DateTimeHelper.StringToDateTimeMethodInfo);
                        }
                        else
                        {
                            var parseMethod = propertyType.GetMethod("Parse",
                                BindingFlags.Static | BindingFlags.Public, null, [typeof(string)], null);
                            ilGen.Emit(OpCodes.Call, parseMethod);
                        }

                        ilGen.Emit(OpCodes.Box, propertyType);
                    }
                }
            }

            ilGen.Emit(OpCodes.Call, ConstExpressionCreateMethod);
            ilGen.Emit(OpCodes.Stloc, 0);

            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldc_I4, i);
            ilGen.Emit(OpCodes.Ldloc, 0);
            ilGen.Emit(OpCodes.Stelem_Ref);
        }

        ilGen.Emit(OpCodes.Ret);
        return (Action<string[], Expression[]>)dynamicMethod.CreateDelegate(typeof(Action<string[], Expression[]>));
    }

    [ILEmitter]
    private static void EmitCommonILForPropertyValueToString(PropertyInfo property, ILGenerator ilGen)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        var propertyType = property.PropertyType;

        ilGen.Emit(OpCodes.Ldarg_0);
        ilGen.Emit(OpCodes.Callvirt, property.GetMethod);

        if (propertyType != typeof(string))
        {
            if (propertyType.IsEnum)
            {
                ilGen.Emit(OpCodes.Box, propertyType);
                ilGen.Emit(OpCodes.Callvirt, StringHelper.ObjectToStringMethod);
            }
            else
            {
                if (propertyType.IsValueType)
                {
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var local = ilGen.DeclareLocal(propertyType);
                        ilGen.Emit(OpCodes.Stloc, local);

                        ilGen.Emit(OpCodes.Ldloca, local);

                        var rawPropertyType = propertyType.GetGenericArguments()[0];
                        var local1 = ilGen.DeclareLocal(rawPropertyType);

                        var ifNotNull = ilGen.DefineLabel();
                        var ifNull = ilGen.DefineLabel();

                        var hasValueMethod = propertyType.GetProperty(nameof(Nullable<int>.HasValue)).GetMethod;
                        ilGen.Emit(OpCodes.Call, hasValueMethod);
                        ilGen.Emit(OpCodes.Brtrue, ifNotNull);
                        ilGen.Emit(OpCodes.Ldnull);
                        ilGen.Emit(OpCodes.Br, ifNull);

                        ilGen.MarkLabel(ifNotNull);
                        ilGen.Emit(OpCodes.Ldloca, local);
                        var getValueMethod = propertyType.GetProperty(nameof(Nullable<int>.Value)).GetMethod;
                        ilGen.Emit(OpCodes.Call, getValueMethod);
                        if (rawPropertyType == typeof(DateTime))
                        {
                            ilGen.Emit(OpCodes.Call, DateTimeHelper.DateTimeToStringMethodInfo);
                        }
                        else
                        {
                            ilGen.Emit(OpCodes.Stloc, local1);
                            ilGen.Emit(OpCodes.Ldloca, local1);
                            var toStringMethod = rawPropertyType.GetMethod(nameof(ToString),
                                BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
                            ilGen.Emit(OpCodes.Call, toStringMethod);
                        }

                        ilGen.MarkLabel(ifNull);
                    }
                    else
                    {
                        if (propertyType == typeof(DateTime))
                        {
                            ilGen.Emit(OpCodes.Call, DateTimeHelper.DateTimeToStringMethodInfo);
                        }
                        else
                        {
                            var local = ilGen.DeclareLocal(propertyType);
                            ilGen.Emit(OpCodes.Stloc, local);
                            ilGen.Emit(OpCodes.Ldloca, local);

                            var toStringMethod = propertyType.GetMethod(nameof(ToString),
                                BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
                            ilGen.Emit(OpCodes.Call, toStringMethod);
                        }
                    }
                }
                else
                {
                    var elementType = propertyType.FindIEnumerable();
                    if (elementType != null)
                    {
                        var methodInfo =
                            CollectionHelper.EnumerableToStringTemplateMethod.MakeGenericMethod(elementType);
                        ilGen.Emit(OpCodes.Call, methodInfo);
                    }
                    else
                    {
                        var local = ilGen.DeclareLocal(propertyType);
                        ilGen.Emit(OpCodes.Stloc, local);
                        ilGen.Emit(OpCodes.Ldloca, local);

                        ilGen.Emit(OpCodes.Call, ConvertHelper.ConvertToStringMethod);
                    }
                }
            }
        }
    }

    private static void GenerateForBuildActionForToStringArray(IEnumerable<PropertyInfo> properties,
        ILGenerator ilGen)
    {
        var indexer = 0;
        foreach (var property in properties)
        {
            ilGen.Emit(OpCodes.Ldarg, 1);
            ilGen.Emit(OpCodes.Ldc_I4, indexer);
            EmitCommonILForPropertyValueToString(property, ilGen);
            ilGen.Emit(OpCodes.Stelem_Ref);
            indexer++;
        }

        ilGen.Emit(OpCodes.Ret);
    }
}