using SubhadraSolutions.Utils.Execution.Contracts;
using System.Data;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.AgentExtensions;

public class DataReaderToDataTableExtension : IAgentExtension<IDataReader>
{
    public string OutputName { get; set; }

    public Task<AgentStatus> Execute(IContext context, IDataReader input)
    {
        return Task.Factory.StartNew(() =>
        {
            var dataTable = new DataTable(OutputName);
            dataTable.Load(input);
            lock (context)
            {
                context.DataTables.Remove(OutputName);
                context.DataTables.Add(OutputName, dataTable);
            }

            return AgentStatus.ExecutedSuccessfully;
        });
    }
}