using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataRecordToDictionaryExtension : IAgentExtension<IDataRecord>
{
    public string OutputName { get; set; }

    public Task<AgentStatus> Execute(IContext context, IDataRecord input)
    {
        var dictionary = new Dictionary<string, object>();
        for (var i = 0; i < input.FieldCount; i++)
        {
            var fieldName = input.GetName(i);
            var value = input[i];
            if (input.IsDBNull(i))
            {
                value = null;
            }

            dictionary.Add(fieldName, value);
        }

        lock (context)
        {
            context.Objects.Remove(OutputName);
            context.Objects.Add(OutputName, dictionary);
        }

        return Task.FromResult(AgentStatus.ExecutedSuccessfully);
    }
}