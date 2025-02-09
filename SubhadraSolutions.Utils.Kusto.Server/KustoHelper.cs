using Kusto.Data.Common;
using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections;

namespace SubhadraSolutions.Utils.Kusto.Server;

public static class KustoHelper
{
    static KustoHelper()
    {
        InitializeClientRequestProperties(DefaultClientRequestProperties);
    }

    public static ClientRequestProperties DefaultClientRequestProperties { get; } = new();

    public static IEnumerator BuildAnonymousEnumerator(ICslQueryProvider cslQueryProvider, string query,
        ClientRequestProperties clientRequestProperties, out Type dtoType)
    {
        var dataReader = cslQueryProvider.ExecuteQuery(query, clientRequestProperties);
        dtoType = dataReader.CreateTypeForDTO();
        var enumerator = Activator.CreateInstance(typeof(DataReaderToEnumeratorAdapter<>).MakeGenericType(dtoType),
            dataReader, null);
        return (IEnumerator)enumerator;
    }

    //public static ClientRequestProperties BuildClientRequestProperties(IEnumerable<KeyValuePair<string, string>> properties)
    //{
    //    var clientRequestProperties = new ClientRequestProperties(new Dictionary<string, object>(), properties);
    //    InitializeClientRequestProperties(clientRequestProperties);
    //    return clientRequestProperties;
    //}

    public static ClientRequestProperties BuildClientRequestProperties()
    {
        var clientRequestProperties = new ClientRequestProperties();
        InitializeClientRequestProperties(clientRequestProperties);
        return clientRequestProperties;
    }

    public static string GetKustoMethodToConvertFromDynamicTo(Type type)
    {
        if (type.IsDateOrTimeType())
        {
            return "todatetime";
        }

        if (type == typeof(int) || type == typeof(int?))
        {
            return "toint";
        }

        if (type == typeof(long) || type == typeof(long?))
        {
            return "tolong";
        }

        if (type == typeof(double) || type == typeof(double?))
        {
            return "todouble";
        }

        if (type == typeof(Guid) || type == typeof(Guid?))
        {
            return "toguid";
        }

        if (type == typeof(string))
        {
            return "tostring";
        }

        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
        {
            return "totimespan";
        }

        if (type == typeof(decimal) || type == typeof(decimal?))
        {
            return "todecimal";
        }

        if (type == typeof(bool) || type == typeof(bool?))
        {
            return "tobool";
        }

        return null;
    }

    public static string GetKustoScalarTypeName(Type type)
    {
        if (type.IsDateOrTimeType())
        {
            return "datetime";
        }

        if (type == typeof(int) || type == typeof(int?))
        {
            return "int";
        }

        if (type == typeof(short) || type == typeof(short?))
        {
            return "int";
        }

        if (type == typeof(byte) || type == typeof(byte?))
        {
            return "int";
        }

        if (type == typeof(long) || type == typeof(long?))
        {
            return "long";
        }

        if (type == typeof(double) || type == typeof(double?))
        {
            return "real";
        }

        if (type == typeof(Guid) || type == typeof(Guid?))
        {
            return "guid";
        }

        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
        {
            return "timespan";
        }

        if (type == typeof(decimal) || type == typeof(decimal?))
        {
            return "decimal";
        }

        if (type == typeof(bool) || type == typeof(bool?))
        {
            return "bool";
        }

        return "dynamic";
    }

    private static void InitializeClientRequestProperties(ClientRequestProperties clientRequestProperties)
    {
        clientRequestProperties.SetOption(ClientRequestProperties.OptionNoTruncation, true);
        clientRequestProperties.SetOption(ClientRequestProperties.OptionNoRequestTimeout, true);
    }
}