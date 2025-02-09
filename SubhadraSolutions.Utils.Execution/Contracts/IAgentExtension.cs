using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Contracts;

public interface IAgentExtension<in T>
{
    Task<AgentStatus> Execute(IContext context, T input);
}