using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SubhadraSolutions.Utils;

public static class EnumHelper
{
    /// <summary>
    ///    Gets the attribute of type T for the enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumVal"></param>
    /// <returns></returns>
    public static T GetAttributeForEnum<T>(object enumVal) where T : Attribute
    {
        var enumType = enumVal.GetType();
        var memInfo = enumType.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }

    public static string GetEnumShortTitle<E>(this E enumVal) where E : Enum
    {
        var attribute = enumVal.GetAttribute<E, DisplayAttribute>();
        if (attribute == null)
        {
            return enumVal.ToString();
        }

        return attribute.ShortName ?? attribute.Name ?? enumVal.ToString();
    }

    public static string GetEnumShortTitle(this object enumVal)
    {
        var attribute = GetAttributeForEnum<DisplayAttribute>(enumVal);
        if (attribute == null)
        {
            return enumVal.ToString();
        }

        return attribute.ShortName ?? attribute.Name ?? enumVal.ToString();
    }

    public static string GetEnumTitle<E>(this E enumVal) where E : Enum
    {
        var attribute = enumVal.GetAttribute<E, DisplayAttribute>();
        if (attribute == null)
        {
            return enumVal.ToString();
        }

        return attribute.Name ?? enumVal.ToString();
    }

    public static string GetEnumTitle(this object enumVal)
    {
        var attribute = GetAttributeForEnum<DisplayAttribute>(enumVal);
        if (attribute == null)
        {
            return enumVal.ToString();
        }

        return attribute.Name ?? enumVal.ToString();
    }

    public static string GetEnumDescription(this object enumVal)
    {
        var attribute = GetAttributeForEnum<DescriptionAttribute>(enumVal);
        if (attribute == null)
        {
            return enumVal.ToString();
        }

        return attribute.Description ?? enumVal.ToString();
    }

    public static string ToDescription(this Enum value)
    {
        var array = (DescriptionAttribute[])value.GetType().GetField(value.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (array.Length != 0)
        {
            return array[0].Description;
        }

        return value.ToString();
    }

    public static string ToDescriptionIfDefined(this Enum value)
    {
        string result = null;
        if (value.GetType().GetEnumValues().Cast<Enum>()
            .Contains(value))
        {
            result = value.ToDescription();
        }

        return result;
    }

    public static HashSet<T> Unflag<F, T>(F flagsEnum, Func<T, F> toFlagsFunc)
        where F : Enum
        where T : Enum
    {
        var result = new HashSet<T>();
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            var flag = toFlagsFunc(value);
            if (((int)(object)flagsEnum & (int)(object)flag) != 0)
            {
                result.Add(value);
            }
        }

        return result;
    }
}