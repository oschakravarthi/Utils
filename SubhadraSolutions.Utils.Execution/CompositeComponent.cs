using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Contracts;
using SubhadraSolutions.Utils.Json;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Execution;

public class CompositeComponent(IServiceProvider serviceProvider) : AbstractComponent, IRunnable
{
    private readonly Dictionary<string, IComponent> components = [];

    public int NumberOfThreads { get; set; }

    public void Run()
    {
        if (NumberOfThreads < 2)
        {
            foreach (var kvp in components)
            {
                var component = kvp.Value;
                if (component is IRunnable runnable)
                {
                    runnable.Run();
                }
            }
        }
        else
        {
            var parallelExecutor = new ParallelExecutor(NumberOfThreads, false);

            foreach (var kvp in components)
            {
                var component = kvp.Value;
                if (component is IRunnable runnable)
                {
                    parallelExecutor.AddAction(new ActionInfo(runnable.Run, 0));
                }
            }

            parallelExecutor.ExecuteAll();
        }
    }

    public void AddComponent(IComponent component)
    {
        components.Add(component.Name, component);
    }

    public void AddComponent(JObject componentState)
    {
        var component = componentState.BuildObject<IComponent>(serviceProvider);
        AddComponent(component);
    }

    protected override void InitializeProtected()
    {
        foreach (var kvp in components)
        {
            var component = kvp.Value;
            if (component is IInitializable { IsInitialized: false } initializable)
            {
                initializable.Initialize();
            }
        }
    }
}