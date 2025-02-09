using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Threading;

public sealed class ParallelExecutor(int degreeOfParallelism, bool useDedicatedThreads)
{
    private readonly List<ActionInfo> actions = [];

    private readonly int degreeOfParallelism = Math.Max(degreeOfParallelism, 1);

    public void AddAction(ActionInfo actionInfo)
    {
        lock (actions)
        {
            actions.Add(actionInfo);
        }
    }

    public void ExecuteAll()
    {
        List<ActionInfo> actionsBackup;

        lock (actions)
        {
            actionsBackup = new List<ActionInfo>(actions);
            actions.Clear();
        }

        ExecuteAllCore(actionsBackup);
    }

    public Task ExecuteAllAsync()
    {
        List<ActionInfo> actionsBackup;

        lock (actions)
        {
            actionsBackup = new List<ActionInfo>(actions);
            actions.Clear();
        }

        return Task.Factory.StartNew(() => ExecuteAllCore(actionsBackup));
    }

    private static void ExecuteOne(ActionData actionData)
    {
        Exception exception = null;
        var actionInfo = actionData.ActionInfo;

        try
        {
            for (long executionCount = 0; executionCount <= actionInfo.MaximumRetryCount; executionCount++)
                try
                {
                    actionInfo.Action();
                    exception = null;
                    Debug.WriteLine($"Action Succeeded: {actionInfo.Name}.");
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Debug.WriteLine($"Action Failed: {actionInfo.Name}. Exception:{ex.ToDetailedString()}");
                    if (executionCount == actionInfo.MaximumRetryCount)
                    {
                        return;
                    }

                    if (actionInfo.RetrySleepTime > 0)
                    {
                        Thread.Sleep(actionInfo.RetrySleepTime);
                    }

                    Debug.WriteLine("Retrying...");
                }
        }
        finally
        {
            if (exception != null)
            {
                actionData.Exeptions.Add(exception);
            }

            actionData.Semaphore.Release();
            actionData.CountdownEvent.Signal();
        }
    }

    private void ExecuteAllCore(IList<ActionInfo> actions)
    {
        var actionsCount = actions.Count;
        using var semaphore = new Semaphore(degreeOfParallelism, degreeOfParallelism);
        using var countdownEvent = new CountdownEvent(actionsCount);
        var exceptions = new List<Exception>();

        for (var i = 0; i < actionsCount; i++)
        {
            var action = actions[i];
            semaphore.WaitOne();
            var actionData = new ActionData(action, semaphore, countdownEvent, exceptions);

            if (useDedicatedThreads)
            {
                var thread = new Thread(() => ExecuteOne(actionData));

                thread.Start();
            }
            else
            {
                var task = Task.Factory.StartNew(() => ExecuteOne(actionData));
            }
        }

        countdownEvent.Wait();
        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }

    private sealed class ActionData(ActionInfo actionInfo, Semaphore semaphore, CountdownEvent countdownEvent,
        List<Exception> exceptions)
    {
        public ActionInfo ActionInfo { get; } = actionInfo;
        public CountdownEvent CountdownEvent { get; } = countdownEvent;
        public List<Exception> Exeptions { get; } = exceptions;
        public Semaphore Semaphore { get; } = semaphore;
    }
}