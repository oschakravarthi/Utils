using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Threading.Tasks;

public class TaskSpooler<TItem>
{
    private readonly int degreeOfParallelism;
    private readonly Func<TItem, Task> taskBuilderAndStarter;

    private readonly ConcurrentQueue<TItem> itemsQueue = new();

    public TaskSpooler(int degreeOfParallelism, Func<TItem, Task> taskBuilderAndStarter)
        : this(degreeOfParallelism, taskBuilderAndStarter, TimeSpan.FromMilliseconds(1))
    {
    }

    public TaskSpooler(int degreeOfParallelism, Func<TItem, Task> taskBuilderAndStarter, TimeSpan waitTimeOnTask)
    {
        this.degreeOfParallelism = Math.Max(degreeOfParallelism, 1);
        this.taskBuilderAndStarter = taskBuilderAndStarter;
        this.WaitTimeOnTask = TimeSpan.FromMilliseconds(Math.Max(waitTimeOnTask.TotalMilliseconds, 1));
    }

    private readonly LinkedList<ItemTaskContext<TItem>> taskLoop = new();

    private long itemsCount = 0;
    public long CompletedItemsCount { get; private set; }
    public long ItemsCount => this.itemsCount;

    public TimeSpan WaitTimeOnTask { get; private set; }

    public event EventHandler<GenericEventArgs<ItemTaskContext<TItem>>> OnProgress;

    private volatile bool isLocked = false;

    //private readonly ManualResetEvent resetEvent = new(true);
    public void AddItem(TItem item)
    {
        this.itemsQueue.Enqueue(item);
        Interlocked.Increment(ref itemsCount);
    }

    public void LockAddingItems()
    {
        this.isLocked = true;
    }

    public Task RunAsync()
    {
        return Task.Run(() => Run());
    }

    private void Run()
    {
        var waitTimeOnTask = (int)this.WaitTimeOnTask.TotalMilliseconds;
        while (true)
        {
            PushItemsToTaskLoop();
            while (taskLoop.Count > 0)
            {
                var context = taskLoop.First.Value;
                taskLoop.RemoveFirst();
                bool bTaskComplete;

                try
                {
                    bTaskComplete = context.Task.Wait(waitTimeOnTask);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                    bTaskComplete = true;
                }

                if (bTaskComplete)
                {
                    CompletedItemsCount++;
                    FireEvent(context);
                }

                PushItemsToTaskLoop();

                if (!bTaskComplete)
                {
                    taskLoop.AddLast(context);
                }
            }
            if (this.isLocked)
            {
                return;
            }
            //Thread.Sleep(waitTimeOnTask);
            Thread.Yield();
        }
    }

    private void FireEvent(ItemTaskContext<TItem> context)
    {
        if (OnProgress != null)
        {
            var args = new GenericEventArgs<ItemTaskContext<TItem>>(context);
            OnProgress(this, args);
        }
    }

    private void PushItemsToTaskLoop()
    {
        while (this.itemsQueue.TryDequeue(out var item))
        {
            var task = taskBuilderAndStarter(item);
            var context = new ItemTaskContext<TItem>(item, task);
            taskLoop.AddLast(context);
            if (taskLoop.Count >= degreeOfParallelism)
            {
                break;
            }
        }
    }
}