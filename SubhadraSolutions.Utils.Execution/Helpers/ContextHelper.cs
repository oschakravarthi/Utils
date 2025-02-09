using SubhadraSolutions.Utils.Execution.Contracts;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Execution.Helpers;

public static class ContextHelper
{
    public static Dictionary<string, object> BuildDictionaryToExport(IContext context)
    {
        var dictionary = new Dictionary<string, object> { { "Properties", context.Arguments } };
        foreach (var kvp in context.Objects)
        {
            dictionary.Add(kvp.Key, kvp.Value);
        }

        foreach (var kvp in context.DataTables)
        {
            dictionary.Add(kvp.Key, kvp.Value);
        }

        foreach (var kvp in context.Collections)
        {
            dictionary.Add(kvp.Key, kvp.Value);
        }

        return dictionary;
    }

    public static void EnsureRequiredParametersExist(IDictionary<string, object> arguments,
        IEnumerable<ParameterDefinition> parameterDefinitions, string agentName)
    {
        if (!HasAllTheRequiredArguments(arguments, parameterDefinitions, out var missingParameter))
        {
            var message = $"No argument for the parameter {missingParameter.Name} for the Agent {agentName}";
            throw new ArgumentException(message, nameof(arguments));
        }
    }

    public static List<ParameterDefinition> GetMissingParameters(IDictionary<string, object> arguments,
        IEnumerable<ParameterDefinition> parameterDefinitions)
    {
        var result = new List<ParameterDefinition>();
        foreach (var parameterDefinition in parameterDefinitions)
        {
            var hasKey = arguments.TryGetValue(parameterDefinition.Name, out var value);
            if (!hasKey || value == null)
            {
                result.Add(parameterDefinition);
            }
        }

        return result;
    }

    public static HashSet<ParameterDefinition> GetParameters(IEnumerable<IAgent> agents)
    {
        var properties = new HashSet<ParameterDefinition>();
        var dictionary = new Dictionary<string, ParameterDefinition>();
        foreach (var child in agents)
        {
            var childParameterDefinitions = child.GetParameterDefinitions();
            foreach (var parameterDefinition in childParameterDefinitions)
            {
                if (dictionary.TryGetValue(parameterDefinition.Name, out var existing))
                {
                    if (existing.Type != parameterDefinition.Type)
                    {
                        throw new ArgumentException("Parameter already exists with a different Type",
                            parameterDefinition.Name);
                    }

                    continue;
                }

                dictionary.Add(parameterDefinition.Name, parameterDefinition);
                properties.Add(parameterDefinition);
            }
        }

        return properties;
    }

    public static bool HasAllTheRequiredArguments(IDictionary<string, object> arguments,
        IEnumerable<ParameterDefinition> parameterDefinitions)
    {
        return HasAllTheRequiredArguments(arguments, parameterDefinitions, out _);
    }

    public static bool HasValidArgument(IDictionary<string, object> arguments, ParameterDefinition parameterDefinition)
    {
        var hasKey = arguments.TryGetValue(parameterDefinition.Name, out var value);
        if (!hasKey || value == null)
        {
            return false;
        }

        var valueAsString = Convert.ToString(value);
        return !string.IsNullOrEmpty(valueAsString);
    }

    private static bool HasAllTheRequiredArguments(IDictionary<string, object> arguments,
        IEnumerable<ParameterDefinition> parameterDefinitions, out ParameterDefinition missingParameter)
    {
        missingParameter = null;
        foreach (var parameterDefinition in parameterDefinitions)
        {
            if (!HasValidArgument(arguments, parameterDefinition))
            {
                missingParameter = parameterDefinition;
                return false;
            }
        }

        return true;
    }
}