using SubhadraSolutions.Utils.Execution.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataReaderToHashSetPropertyExtension : IAgentExtension<IDataReader>
{
    private static readonly MethodInfo BuildHashSetTemplateMethod =
        typeof(DataReaderToHashSetPropertyExtension).GetMethod(nameof(BuildHashSet),
            BindingFlags.Static | BindingFlags.NonPublic);

    public string FieldName { get; set; }
    public string PropertyName { get; set; }

    public Task<AgentStatus> Execute(IContext context, IDataReader input)
    {
        return Task.Factory.StartNew(() =>
        {
            var fieldName = string.IsNullOrEmpty(FieldName) ? PropertyName : FieldName;
            var propertyName = string.IsNullOrEmpty(PropertyName) ? FieldName : PropertyName;

            var fieldIndex = input.GetOrdinal(fieldName);
            var fieldType = input.GetFieldType(fieldIndex);

            var method = BuildHashSetTemplateMethod.MakeGenericMethod(fieldType);
            var set = method.Invoke(null, [input, fieldIndex]);
            context.Arguments[propertyName] = set;
            return AgentStatus.ExecutedSuccessfully;
        });
    }

    private static HashSet<T> BuildHashSet<T>(IDataReader reader, int fieldIndex)
    {
        var set = new HashSet<T>();
        while (reader.Read())
        {
            var obj = (T)reader[fieldIndex];
            if (obj != null && !string.IsNullOrEmpty(Convert.ToString(obj)))
            {
                set.Add(obj);
            }
        }

        return set;
    }
}