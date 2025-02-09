using SubhadraSolutions.Utils.Data;
using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Agents;

public abstract class AbstractQueryAgent : AbstractExplicitParametersAgent
{
    protected IAgentExtension<IDataReader> extension;

    public List<IDataReaderDecorator> DataReaderDecorators { get; set; } = [];

    protected override Task<AgentStatus> ExecuteCore(IContext context)
    {
        return Task.Factory.StartNew(() =>
        {
            var arguments = context.AgentArguments[Name];
            var dataReaderTask = GetDataReaderAsync(context, arguments);
            var dataReader = dataReaderTask.Result;
            if (DataReaderDecorators != null)
            {
                foreach (var decorator in DataReaderDecorators)
                {
                    decorator.Actual = dataReader;
                    dataReader = decorator;
                }
            }

            //dataReader = new DataReaderInternDecorator(dataReader);
            var task = extension.Execute(context, dataReader);
            task.Wait();
            dataReader.Close();
            return task.Result;
        });
    }

    protected abstract Task<IDataReader> GetDataReaderAsync(IContext context, IDictionary<string, object> arguments);
}