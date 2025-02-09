using SubhadraSolutions.Utils.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Contracts;

public interface IAgent : INamed
{
    bool Enabled { get; }

    Task<AgentStatus> Execute(IContext context, bool silent);

    HashSet<ParameterDefinition> GetParameterDefinitions();
}