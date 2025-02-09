using System;

namespace SubhadraSolutions.Utils;

public static class TypeExtensions
{
    public static TypeCode GetTypeCode(this Type type)
    {
        if ((object)type == null)
        {
            return TypeCode.Empty;
        }

        if ((object)type == typeof(bool))
        {
            return TypeCode.Boolean;
        }

        if ((object)type == typeof(byte))
        {
            return TypeCode.Byte;
        }

        if ((object)type == typeof(char))
        {
            return TypeCode.Char;
        }

        if ((object)type == typeof(ushort))
        {
            return TypeCode.UInt16;
        }

        if ((object)type == typeof(uint))
        {
            return TypeCode.UInt32;
        }

        if ((object)type == typeof(ulong))
        {
            return TypeCode.UInt64;
        }

        if ((object)type == typeof(sbyte))
        {
            return TypeCode.SByte;
        }

        if ((object)type == typeof(short))
        {
            return TypeCode.Int16;
        }

        if ((object)type == typeof(int))
        {
            return TypeCode.Int32;
        }

        if ((object)type == typeof(long))
        {
            return TypeCode.Int64;
        }

        if ((object)type == typeof(string))
        {
            return TypeCode.String;
        }

        if ((object)type == typeof(float))
        {
            return TypeCode.Single;
        }

        if ((object)type == typeof(double))
        {
            return TypeCode.Double;
        }

        if ((object)type == typeof(DateTime))
        {
            return TypeCode.DateTime;
        }

        if ((object)type == typeof(decimal))
        {
            return TypeCode.Decimal;
        }

        return TypeCode.Object;
    }

    //public static Assembly GetAssembly(this Type type)
    //{
    //    return type?.GetTypeInfo()?.get_Assembly();
    //}
}