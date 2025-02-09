using SubhadraSolutions.Utils.Execution.Contracts;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataReaderToPropertiesExtension : IAgentExtension<IDataReader>
{
    public bool ExcludeNulls { get; set; } = false;
    public bool OverwriteExisting { get; set; } = false;

    public Task<AgentStatus> Execute(IContext context, IDataReader input)
    {
        return Task.Factory.StartNew(() =>
        {
            var hasRecord = input.Read();
            if (!hasRecord)
            {
                return AgentStatus.ExecutedSuccessfully;
            }

            var extension = new DataRecordToPropertiesExtension
            {
                OverwriteExisting = OverwriteExisting,
                ExcludeNulls = ExcludeNulls
            };
            var task = extension.Execute(context, input);
            task.Wait();
            return task.Result;
        });
    }
}