using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Reflection;

namespace SubhadraSolutions.Utils;

public static class NumberHelper
{
    public static readonly MethodInfo MinifyDecimalMethod =
        typeof(NumberHelper).GetMethod(nameof(MinifyDecimal), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo MinifyDoubleMethod =
        typeof(NumberHelper).GetMethod(nameof(MinifyDouble), BindingFlags.Public | BindingFlags.Static);

    public static readonly MethodInfo MinifyInt32Method =
        typeof(NumberHelper).GetMethod(nameof(MinifyInt), BindingFlags.Public | BindingFlags.Static);

    public static bool IsValidNumber(double value)
    {
        if (double.IsNaN(value))
        {
            return false;
        }

        if (double.IsInfinity(value))
        {
            return false;
        }

        if (double.IsNegativeInfinity(value))
        {
            return false;
        }

        return true;
    }

    public static string Minify(int value, int numberofDecimalPlaces)
    {
        return Minify((double)value, numberofDecimalPlaces);
    }

    public static string Minify(decimal value, int numberofDecimalPlaces)
    {
        return Minify((double)value, numberofDecimalPlaces);
    }

    public static string Minify(double value, int numberofDecimalPlaces)
    {
        if (double.IsNaN(value))
        {
            return "NaN";
        }

        if (double.IsInfinity(value))
        {
            return "∞";
        }

        if (double.IsNegativeInfinity(value))
        {
            return "-∞";
        }

        var multiplier = value < 0 ? -1 : 1;
        value = Math.Abs(value);

        if (value >= 1000000000)
        {
            return Math.Round(value * multiplier / 1000000000, numberofDecimalPlaces) + " B";
        }

        if (value >= 1000000)
        {
            return Math.Round(value * multiplier / 1000000, numberofDecimalPlaces) + " M";
        }

        if (value >= 1000)
        {
            return Math.Round(value * multiplier / 1000, numberofDecimalPlaces) + " K";
        }

        return Math.Round(value * multiplier, numberofDecimalPlaces).ToString();
    }

    [DynamicallyInvoked]
    public static string MinifyDecimal(decimal value)
    {
        return MinifyDouble((double)value);
    }

    [DynamicallyInvoked]
    public static string MinifyDouble(double value)
    {
        return Minify(value, 2);
    }

    public static string MinifyInt(int value)
    {
        return MinifyDouble(value);
    }
}