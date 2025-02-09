using Kusto.Data.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace SubhadraSolutions.Utils.Kusto.Server.Linq;

internal class QueryContext
{
    private readonly StringBuilder parametersSectionStringBuilder = new();

    public ClientRequestProperties ClientRequestProperties { get; } = KustoHelper.BuildClientRequestProperties();

    public Dictionary<Expression, HashSet<string>> ExistingMembers { get; } = [];

    public Dictionary<Expression, string> ParameterAliases { get; } = [];

    public Sequencer Sequencer { get; private set; } = new();

    public string AddParameter(string value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(string));
        return parameterName;
    }

    public string AddParameter(TimeSpan value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(TimeSpan));
        return parameterName;
    }

    public string AddParameter(bool value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(bool));
        return parameterName;
    }

    public string AddParameter(DateTime value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(DateTime));
        return parameterName;
    }

    public string AddParameter(double value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(double));
        return parameterName;
    }

    public string AddParameter(Guid value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(Guid));
        return parameterName;
    }

    public string AddParameter(int value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(int));
        return parameterName;
    }

    public string AddParameter(char value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value.ToString());
        AddParameterToSection(parameterName, typeof(int));
        return parameterName;
    }

    public string AddParameter(long value)
    {
        var parameterName = GetParameterName();
        ClientRequestProperties.SetParameter(parameterName, value);
        AddParameterToSection(parameterName, typeof(long));
        return parameterName;
    }

    public string BuildQueryParametersHeader()
    {
        if (parametersSectionStringBuilder.Length == 0)
        {
            return null;
        }

        var result = $"declare query_parameters({parametersSectionStringBuilder});";
        return result;
    }

    public void PrintParameters()
    {
        var parameters = ClientRequestProperties.Parameters;
        if (parameters == null)
        {
            return;
        }

        foreach (var kvp in parameters)
        {
            Debug.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
        }
    }

    private void AddParameterToSection(string parameterName, Type type)
    {
        var kustoType = KustoHelper.GetKustoScalarTypeName(type);
        if (parametersSectionStringBuilder.Length > 0)
        {
            parametersSectionStringBuilder.Append(',');
        }

        parametersSectionStringBuilder.Append($"{parameterName}:{kustoType}");
    }

    private string GetParameterName()
    {
        var count = ClientRequestProperties.Parameters == null ? 0 : ClientRequestProperties.Parameters.Count;
        var parameterName = "P__" + count;
        return parameterName;
    }
}