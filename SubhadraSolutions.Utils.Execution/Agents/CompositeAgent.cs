using SubhadraSolutions.Utils.Execution.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Agents;

public sealed class CompositeAgent : AbstractCompositeAgent
{
    public bool Async { get; set; }

    //public List<ParameterDefinition> ParameterDefinitions { get { return new List<ParameterDefinition>(); } }
    public override Task<AgentStatus> Execute(IContext context, bool silent)
    {
        InitializeArguments(context);
        var arguments = context.AgentArguments[Name];
        var shouldProceed = PreExecute(context, silent);
        if (!shouldProceed)
        {
            return Task.FromResult(AgentStatus.ExecutionCancelled);
        }

        if (Async)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (!child.Enabled)
                {
                    continue;
                }

                context.AgentArguments[child.Name] = arguments;
                var task = child.Execute(context, true);
                tasks.Add(task);
            }

            try
            {
                var allTask = Task.WhenAll(tasks);
                allTask.Wait();
            }
            catch (AggregateException ae)
            {
                System.Console.WriteLine(ae.ToString());
                return Task.FromResult(AgentStatus.ExecutionErrored);
            }
        }
        else
        {
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (!child.Enabled)
                {
                    continue;
                }

                context.AgentArguments[child.Name] = arguments;
                var task = child.Execute(context, true);

                try
                {
                    task.Wait();
                }
                catch (AggregateException ae)
                {
                    System.Console.WriteLine(ae.ToString());
                    return Task.FromResult(AgentStatus.ExecutionErrored);
                }
            }
        }

        return Task.FromResult(AgentStatus.ExecutedSuccessfully);
    }

    protected override Task<AgentStatus> ExecuteCore(IContext context)
    {
        return Task.FromResult(AgentStatus.None);
    }
}