using SubhadraSolutions.Utils.Execution.Abstractions;
using SubhadraSolutions.Utils.Execution.Contracts;
using SubhadraSolutions.Utils.Execution.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Execution.Agents;

public abstract class AbstractAgent : AbstractNamedDescriptioned, IAgent
{
    public bool Enabled { get; set; } = true;

    public virtual Task<AgentStatus> Execute(IContext context, bool silent)
    {
        InitializeArguments(context);
        var arguments = context.AgentArguments[Name];
        if (context.AgentStatuses.TryGetValue(Name, out var agentSt))
        {
            if (agentSt == AgentStatus.InExecution)
            {
                return Task.FromResult(agentSt);
            }
        }

        var canProceedWithExecution = PreExecute(context, silent);

        //context.AgentStatuses[this.Name] = AgentStatus.SetToBeExecuted;
        if (!canProceedWithExecution)
        {
            context.AgentStatuses[Name] = AgentStatus.ExecutionCancelled;
            return Task.FromResult(AgentStatus.ExecutionCancelled);
        }

        context.AgentStatuses[Name] = AgentStatus.InExecution;
        arguments = context.AgentArguments[Name];
        var task = ExecuteCore(context);
        task = task.ContinueWith(antecedent =>
        {
            AgentStatus agentStatus;
            try
            {
                agentStatus = antecedent.Result;
            }
            catch (AggregateException ae)
            {
                System.Console.WriteLine(ae.ToString());
                agentStatus = AgentStatus.ExecutionErrored;
            }

            context.AgentStatuses[Name] = agentStatus;
            return agentStatus;
        }, TaskContinuationOptions.ExecuteSynchronously);
        return task;
    }

    public abstract HashSet<ParameterDefinition> GetParameterDefinitions();

    protected abstract Task<AgentStatus> ExecuteCore(IContext context);

    protected void InitializeArguments(IContext context)
    {
        if (!context.AgentArguments.TryGetValue(Name, out var arguments))
        {
            arguments = new Dictionary<string, object>();
        }

        var parameterDefinitions = GetParameterDefinitions();
        foreach (var parameterDefinition in parameterDefinitions)
        {
            if (!arguments.ContainsKey(parameterDefinition.Name))
            {
                if (context.Arguments.TryGetValue(parameterDefinition.Name, out var value))
                {
                    arguments.Add(parameterDefinition.Name, value);
                }
            }
        }

        context.AgentArguments[Name] = arguments;
    }

    protected virtual bool PreExecute(IContext context, bool silent)
    {
        var arguments = context.AgentArguments[Name];
        if (!Enabled)
        {
            return false;
        }

        var parameterDefinitions = GetParameterDefinitions();
        var callback = context.ArgumentsNeededCallback;
        if (!silent && callback != null)
        {
            callback(parameterDefinitions, arguments);
        }

        var missingParameters = ContextHelper.GetMissingParameters(arguments, parameterDefinitions);

        if (missingParameters.Count > 0)
        {
            return false;
        }

        return true;
        //return ContextHelper.HasAllTheRequiredArguments(arguments, parameterDefinitions);
    }
}