using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Reflection;

namespace SubhadraSolutions.Utils.Data;

public static class DataRecordHelper
{
    public static readonly MethodInfo IsDbNullMethod =
        typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), BindingFlags.Public | BindingFlags.Instance);

    private static readonly Dictionary<Type, MethodInfo> dataRecordReadMethods = [];

    static DataRecordHelper()
    {
        PopulateDataRecordReadMethods();
    }

    public static int[] CalculateOrdinals(IDataRecord record, string[] columnNames)
    {
        var ordinals = new int[columnNames.Length];

        for (var i = 0; i < columnNames.Length; i++)
        {
            var columnName = columnNames[i];
            ordinals[i] = GetOrdinal(record, columnName);
        }

        return ordinals;
    }

    public static bool CanReadFromRecord(Type type)
    {
        return GetReaderMethod(type) != null;
    }

    public static Type CreateTypeForDTO(this IDataRecord record)
    {
        return record.CreateTypeForDTO(out var _);
    }

    public static Type CreateTypeForDTO(this IDataRecord record, out List<Tuple<string, Type>> tuples)
    {
        tuples = [];
        for (var i = 0; i < record.FieldCount; i++)
            tuples.Add(new Tuple<string, Type>(record.GetName(i), record.GetFieldType(i)));
        var type = AnonymousTypeBuilder.BuildAnonymousType(tuples);
        return type;
    }

    public static int GetOrdinal(IDataRecord record, string columnName)
    {
        for (var i = 0; i < record.FieldCount; i++)
        {
            var field = record.GetName(i);

            if (string.Equals(field, columnName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    public static MethodInfo GetReaderMethod(Type type)
    {
        if (dataRecordReadMethods.TryGetValue(type, out var method))
        {
            return method;
        }

        return null;
    }

    [DynamicallyInvoked]
    public static sbyte GetSByte(IDataRecord record, int index)
    {
        return (sbyte)record[index];
    }

    [DynamicallyInvoked]
    public static SqlBinary GetSqlBinary(IDataRecord record, int index)
    {
        return (SqlBinary)record[index];
    }

    [DynamicallyInvoked]
    public static SqlBoolean GetSqlBoolean(IDataRecord record, int index)
    {
        return (SqlBoolean)record[index];
    }

    [DynamicallyInvoked]
    public static SqlByte GetSqlByte(IDataRecord record, int index)
    {
        return (SqlByte)record[index];
    }

    [DynamicallyInvoked]
    public static SqlBytes GetSqlBytes(IDataRecord record, int index)
    {
        return (SqlBytes)record[index];
    }

    [DynamicallyInvoked]
    public static SqlChars GetSqlChars(IDataRecord record, int index)
    {
        return (SqlChars)record[index];
    }

    [DynamicallyInvoked]
    public static SqlDateTime GetSqlDateTime(IDataRecord record, int index)
    {
        return (SqlDateTime)record[index];
    }

    [DynamicallyInvoked]
    public static SqlDecimal GetSqlDecimal(IDataRecord record, int index)
    {
        return (SqlDecimal)record[index];
    }

    [DynamicallyInvoked]
    public static SqlDouble GetSqlDouble(IDataRecord record, int index)
    {
        return (SqlDouble)record[index];
    }

    [DynamicallyInvoked]
    public static SqlGuid GetSqlGuid(IDataRecord record, int index)
    {
        return (SqlGuid)record[index];
    }

    [DynamicallyInvoked]
    public static SqlInt16 GetSqlInt16l(IDataRecord record, int index)
    {
        return (SqlInt16)record[index];
    }

    [DynamicallyInvoked]
    public static SqlInt32 GetSqlInt32(IDataRecord record, int index)
    {
        return (SqlInt32)record[index];
    }

    [DynamicallyInvoked]
    public static SqlInt64 GetSqlInt64(IDataRecord record, int index)
    {
        return (SqlInt64)record[index];
    }

    [DynamicallyInvoked]
    public static SqlMoney GetSqlMoney(IDataRecord record, int index)
    {
        return (SqlMoney)record[index];
    }

    [DynamicallyInvoked]
    public static SqlSingle GetSqlSingle(IDataRecord record, int index)
    {
        return (SqlSingle)record[index];
    }

    [DynamicallyInvoked]
    public static SqlString GetSqlString(IDataRecord record, int index)
    {
        return (SqlString)record[index];
    }

    [DynamicallyInvoked]
    public static SqlXml GetSqlXml(IDataRecord record, int index)
    {
        return (SqlXml)record[index];
    }

    [DynamicallyInvoked]
    public static TimeSpan GetTimeSpan(IDataRecord record, int index)
    {
        return (TimeSpan)record[index];
    }

    private static void PopulateDataRecordReadMethods()
    {
        foreach (var method in typeof(IDataRecord).GetMethods())
        {
            if (method.Name.StartsWith("Get") && method.Name != "GetDataTypeName" && method.Name != "GetFieldType" &&
                method.Name != "GetName")
            {
                var parameters = method.GetParameters();

                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
                {
                    dataRecordReadMethods.Add(method.ReturnType, method);
                }
            }
        }

        //MethodInfo getSbyteMethod = typeof(DataRecordHelper).GetMethod(nameof(GetSByte), BindingFlags.Static | BindingFlags.Public);
        //dataRecordReadMethods.Add(typeof(sbyte), getSbyteMethod);

        foreach (var method in typeof(DataRecordHelper).GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(IDataRecord) &&
                parameters[1].ParameterType == typeof(int))
            {
                dataRecordReadMethods.Add(method.ReturnType, method);
            }
        }
    }
}