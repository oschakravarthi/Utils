using SubhadraSolutions.Utils.Execution.Abstractions;
using SubhadraSolutions.Utils.Execution.Agents;
using SubhadraSolutions.Utils.Execution.Contracts;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Execution.Playbooks;

public sealed class AgentTreeNode : AbstractNamedDescriptioned
{
    public bool Async { get; set; } = false;
    public List<AgentTreeNode> Children { get; set; } = [];
    public bool PickOne { get; set; } = false;

    public IAgent BuildAgent(IDictionary<string, IAgent> agents)
    {
        if (agents.TryGetValue(Name, out var agent))
        {
            if (Children == null || Children.Count == 0)
            {
                return agent.Enabled ? agent : null;
            }
        }

        AbstractCompositeAgent node;
        if (PickOne)
        {
            node = new PickOneAgent
            {
                Name = Name,
                Description = Description
            };
        }
        else
        {
            node = new CompositeAgent
            {
                Name = Name,
                Description = Description,
                Async = Async
            };
        }

        foreach (var child in Children)
        {
            var childAgent = child.BuildAgent(agents);
            if (childAgent != null)
            {
                node.Children.Add(childAgent);
            }
        }

        return node;
    }
}