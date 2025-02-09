using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SubhadraSolutions.Utils.Reflection;

public static class ConvertHelper
{
    public static readonly MethodInfo ConvertToStringMethod = typeof(Convert).GetMethod(nameof(Convert.ToString),
        BindingFlags.Static | BindingFlags.Public, null, [typeof(object)], null);

    private static readonly Dictionary<KeyValuePair<Type, Type>, MethodInfo> convertToMethods = [];

    static ConvertHelper()
    {
        PopulateConvertToMethods();
    }

    [DynamicallyInvoked]
    public static DateOnly DateTimeToDate(DateTime value)
    {
        return new DateOnly(value.Year, value.Month, value.Day);
    }

    [DynamicallyInvoked]
    public static TimeOnly DateTimeToTime(DateTime value)
    {
        return TimeOnly.FromDateTime(value);
    }

    [DynamicallyInvoked]
    public static DateTime DateToDateTime(DateOnly value)
    {
        return new DateTime(value.Year, value.Month, value.Day);
    }

    public static Func<TFrom, TTo> GetConvertMethod<TFrom, TTo>(bool throwExceptionWhenNotFound = false)
    {
        var method = GetConvertMethod(typeof(TFrom), typeof(TTo), throwExceptionWhenNotFound);
        if (method == null)
        {
            return null;
        }

        return Expression.Lambda<Func<TFrom, TTo>>(Expression.Call(method)).Compile();
    }

    public static MethodInfo GetConvertMethod(Type from, Type to, bool throwExceptionWhenNotFound = false)
    {
        if (convertToMethods.TryGetValue(new KeyValuePair<Type, Type>(from, to), out var method))
        {
            return method;
        }

        var result = JsonSerializationHelper.JsonDeserializeTemplateMethod.MakeGenericMethod(to);
        if (result == null && throwExceptionWhenNotFound)
        {
            throw new MissingMethodException($"Could not find a mathod to convert from type {from} to type {to}");
        }

        return result;
    }

    [DynamicallyInvoked]
    public static byte[] ToBinary(SqlBinary value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static bool ToBoolean(SqlBoolean value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static byte ToByte(SqlByte value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static byte[] ToBytes(SqlBytes value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static char[] ToChars(SqlChars value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static DateTime ToDateTime(SqlDateTime value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static decimal ToDecimal(SqlDecimal value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static decimal ToDecimal(SqlMoney value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static double ToDouble(SqlDouble value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static Guid ToGuid(SqlGuid value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static short ToInt16(SqlInt16 value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static int ToInt32(SqlInt32 value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static long ToInt64(SqlInt64 value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static float ToSingle(SqlSingle value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static string ToString(SqlString value)
    {
        return value.Value;
    }

    [DynamicallyInvoked]
    public static string ToXmlString(SqlXml value)
    {
        return value.Value;
    }

    private static void PopulateConvertToMethods()
    {
        foreach (var method in typeof(Convert).GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (method.Name.StartsWith("To") && method.Name != "ToBase64String")
            {
                var parameters = method.GetParameters();

                if (parameters.Length == 1)
                {
                    convertToMethods.Add(
                        new KeyValuePair<Type, Type>(parameters[0].ParameterType, method.ReturnType), method);
                }
            }
        }

        foreach (var method in typeof(ConvertHelper).GetMethods(BindingFlags.Static | BindingFlags.Public)
                     .Where(m => m.Name.StartsWith("To")))
        {
            convertToMethods.Add(
                new KeyValuePair<Type, Type>(method.GetParameters()[0].ParameterType, method.ReturnType), method);
        }

        convertToMethods.Add(new KeyValuePair<Type, Type>(typeof(DateOnly), typeof(DateTime)),
            typeof(ConvertHelper).GetMethod(nameof(DateToDateTime), BindingFlags.Static | BindingFlags.Public));
        convertToMethods.Add(new KeyValuePair<Type, Type>(typeof(DateTime), typeof(DateOnly)),
            typeof(ConvertHelper).GetMethod(nameof(DateTimeToDate), BindingFlags.Static | BindingFlags.Public));
        convertToMethods.Add(new KeyValuePair<Type, Type>(typeof(DateTime), typeof(TimeOnly)),
            typeof(ConvertHelper).GetMethod(nameof(DateTimeToTime), BindingFlags.Static | BindingFlags.Public));
    }
}