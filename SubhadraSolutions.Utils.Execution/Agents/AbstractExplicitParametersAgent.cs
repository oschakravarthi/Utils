using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Execution.Agents;

public abstract class AbstractExplicitParametersAgent : AbstractAgent, IAgent
{
    public List<ParameterDefinition> ParameterDefinitions { get; set; } = [];

    public override HashSet<ParameterDefinition> GetParameterDefinitions()
    {
        if (ParameterDefinitions == null)
        {
            return [];
        }

        return ParameterDefinitions.ToHashSet();
    }
}