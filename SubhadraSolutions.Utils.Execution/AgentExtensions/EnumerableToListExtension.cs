using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class EnumerableToListExtension<T> : IAgentExtension<IEnumerable<T>>
{
    public string OutputName { get; set; }

    public Task<AgentStatus> Execute(IContext context, IEnumerable<T> input)
    {
        return Task.Factory.StartNew(() =>
        {
            var list = new List<T>(input);
            lock (context)
            {
                context.Collections.Remove(OutputName);
                context.Collections.Add(OutputName, list);
            }

            return AgentStatus.ExecutedSuccessfully;
        });
    }
}