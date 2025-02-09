using SubhadraSolutions.Utils.Execution.Contracts;
using SubhadraSolutions.Utils.Execution.Playbooks;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Runtime;

public class PlaybookPlayer(IContext context)
{
    public Task<AgentStatus> Execute(AgentTreeNode node)
    {
        var agent = node.BuildAgent(context.Agents);
        if (agent == null)
        {
            return Task.FromResult(AgentStatus.ExecutionCancelled);
        }

        var task = agent.Execute(context, false);
        return task;
    }
}