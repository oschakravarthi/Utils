using SubhadraSolutions.Utils.Execution.Contracts;
using SubhadraSolutions.Utils.Execution.Helpers;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Agents;

public class PickOneAgent : AbstractCompositeAgent
{
    //public override Task<AgentStatus> Execute(IContext context)
    //{
    //    var tuples = new List<Tuple<IAgent, List<ParameterDefinition>>>();
    //    for (int i = 0; i < this.Children.Count; i++)
    //    {
    //        var child = this.Children[i];
    //        var agent = context.Agents[child.Name];
    //        var parameters = agent.ParameterDefinitions;
    //        tuples.Add(new Tuple<IAgent, List<ParameterDefinition>>(agent, parameters));
    //    }

    //    tuples.Sort((a, b) => b.Item2.Count.CompareTo(a.Item2.Count));

    //    for (int i = 0; i < tuples.Count; i++)
    //    {
    //        var tuple = tuples[i];
    //        if (ContextHelper.HasAllTheRequiredArguments(context.Arguments, tuple.Item2))
    //        {
    //            return tuple.Item1.Execute(context);
    //        }
    //    }
    //    var task = tuples[0].Item1.Execute(context);
    //    task.Wait();
    //    return Task.FromResult(AgentStatus.None);
    //}

    public override Task<AgentStatus> Execute(IContext context, bool silent)
    {
        InitializeArguments(context);
        var arguments = context.AgentArguments[Name];
        var shouldProceed = PreExecute(context, silent);
        if (!shouldProceed)
        {
            return Task.FromResult(AgentStatus.ExecutionCancelled);
        }

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (!child.Enabled)
            {
                continue;
            }

            var agent = context.Agents[child.Name];
            var parameterDefinitions = agent.GetParameterDefinitions();
            if (ContextHelper.HasAllTheRequiredArguments(context.Arguments, parameterDefinitions))
            {
                context.AgentArguments[agent.Name] = arguments;
                return agent.Execute(context, true);
            }
        }

        return Task.FromResult(AgentStatus.None);
    }

    protected override Task<AgentStatus> ExecuteCore(IContext context)
    {
        return Task.FromResult(AgentStatus.None);
    }
}