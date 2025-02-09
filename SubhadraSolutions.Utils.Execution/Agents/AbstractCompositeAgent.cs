using SubhadraSolutions.Utils.Execution.Contracts;
using SubhadraSolutions.Utils.Execution.Helpers;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Execution.Agents;

public abstract class AbstractCompositeAgent : AbstractAgent, IAgent
{
    public List<IAgent> Children { get; set; } = [];

    public override HashSet<ParameterDefinition> GetParameterDefinitions()
    {
        var properties = ContextHelper.GetParameters(Children);
        return properties;
    }
}