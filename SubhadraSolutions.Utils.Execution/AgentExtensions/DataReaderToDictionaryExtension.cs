using SubhadraSolutions.Utils.Execution.Contracts;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataReaderToDictionaryExtension : IAgentExtension<IDataReader>
{
    public string OutputName { get; set; }

    public Task<AgentStatus> Execute(IContext context, IDataReader input)
    {
        return Task.Factory.StartNew(() =>
        {
            var hasRecord = input.Read();
            if (!hasRecord)
            {
                return AgentStatus.ExecutedSuccessfully;
            }

            var extension = new DataRecordToDictionaryExtension
            {
                OutputName = OutputName
            };
            var task = extension.Execute(context, input);
            task.Wait();
            return task.Result;
        });
    }
}