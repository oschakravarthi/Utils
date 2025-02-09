using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataReaderToEnumerableExtension<T> : IAgentExtension<IDataReader>
{
    public IAgentExtension<IEnumerable<T>> Extension { get; set; }

    public Task<AgentStatus> Execute(IContext context, IDataReader input)
    {
        return Task.Factory.StartNew(() =>
        {
            var enumerable = DynamicDataReaderToEntitiesHelper<T>.MapToEntities(input);
            enumerable = Interner<T>.WrapIntern(enumerable);
            var task = Extension.Execute(context, enumerable);
            task.Wait();
            return task.Result;
        });
    }
}