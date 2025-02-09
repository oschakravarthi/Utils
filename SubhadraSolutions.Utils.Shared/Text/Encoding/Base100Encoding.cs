using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Text.Encoding;

public static class Base100Encoding
{
    public const string BASE_64_DIGITS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    public const string BASE_HUNDRED_DIGITS =
        "!;#$&+0123456789=?@ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz¡£¥§¿ÄÅÆÇÉÑÖØÜßàäåæèéìñòöøùü";

    public static int DecodeBase64ToInt32(string base64String)
    {
        var integer = GetInt64(base64String, BASE_64_DIGITS);

        if (integer > int.MaxValue)
        {
            throw new OverflowException("Number too large for a 32 bit int.");
        }

        return (int)integer;
    }

    public static long DecodeBase64ToInt64(string base64String)
    {
        return GetInt64(base64String, BASE_64_DIGITS);
    }

    public static int DecodeToInt32(string baseHundredNumberString)
    {
        return DecodeBase64ToInt32(baseHundredNumberString);
    }

    public static long DecodeToInt64(string baseHundredNumberString)
    {
        return DecodeBase64ToInt64(baseHundredNumberString);
    }

    public static string Encode(long integer, byte outputLength)
    {
        return EncodeBase64(integer, outputLength);
    }

    public static string Encode(long integer)
    {
        return EncodeBase64(integer);
    }

    public static string EncodeBase64(long integer, byte outputLength)
    {
        if (integer > Math.Pow(BASE_64_DIGITS.Length, outputLength))
        {
            throw new OverflowException("Number specified too big for output length in Base 64.");
        }

        var str = GetString(integer, BASE_64_DIGITS);

        if (str.Length < outputLength)
        {
            return str.PadLeft(outputLength, BASE_64_DIGITS[0]);
        }

        return str;
    }

    public static string EncodeBase64(long integer)
    {
        if (integer < 0)
        {
            throw new Exception("Base 100 number must be positive");
        }

        return GetString(integer, BASE_64_DIGITS);
    }

    private static long GetInt64(string numberString, string digits)
    {
        var numberSystemLength = digits.Length;

        return numberString.Select((t, i) =>
            digits.IndexOf(t) * (long)Math.Pow(numberSystemLength, numberString.Length - i - 1)).Sum();
    }

    private static string GetString(long integer, string digits)
    {
        var numberSystemLength = digits.Length;
        var str = string.Empty;

        while (integer >= numberSystemLength)
        {
            str = digits[(int)(integer % numberSystemLength)] + str;
            integer /= numberSystemLength;
        }

        return digits[(int)(integer % numberSystemLength)] + str;
    }
}