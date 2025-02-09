using SubhadraSolutions.Utils.Execution.Contracts;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataRecordToPropertiesExtension : IAgentExtension<IDataRecord>
{
    public bool ExcludeNulls { get; set; } = false;
    public bool OverwriteExisting { get; set; } = false;

    public Task<AgentStatus> Execute(IContext context, IDataRecord input)
    {
        for (var i = 0; i < input.FieldCount; i++)
        {
            var fieldName = input.GetName(i);
            var value = input[i];
            if (input.IsDBNull(i))
            {
                value = null;
            }
            else
            {
                var s = Convert.ToString(value);
                s = s.Trim();
                if (string.IsNullOrEmpty(s))
                {
                    value = null;
                }

                if (string.Equals(s, "null", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(s, "<null>", StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
            }

            if (!context.Arguments.TryGetValue(fieldName, out var existingValue))
            {
                if (ExcludeNulls && value == null)
                {
                    continue;
                }

                context.Arguments[fieldName] = value;
                continue;
            }

            var fieldType = input.GetFieldType(i);
            var defaultValue = fieldType.GetDefaultValueForType();
            if (value == null && defaultValue == null)
            {
                continue;
            }

            if (value != null)
            {
                if (existingValue == null)
                {
                    context.Arguments[fieldName] = value;
                    continue;
                }

                if (((IComparable)existingValue).CompareTo(defaultValue) == 0)
                {
                    context.Arguments[fieldName] = value;
                    continue;
                }
            }

            if (OverwriteExisting)
            {
                context.Arguments[fieldName] = value;
            }
        }

        return Task.FromResult(AgentStatus.ExecutedSuccessfully);
    }
}