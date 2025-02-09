using System.Threading;

namespace SubhadraSolutions.Utils.Threading;

public class ThreadLimits
{
    public int? CompletionPortThreads { get; set; }

    public int? WorkerThreads { get; set; }
}

public sealed class ThreadPoolSettings
{
    private ThreadPoolSettings()
    {
    }

    public static ThreadPoolSettings Instance { get; } = new();

    public ThreadLimits MaxThreads { get; set; }

    public ThreadLimits MinThreads { get; set; }

    public void Apply()
    {
        if (MinThreads != null && (MinThreads.WorkerThreads != null || MinThreads.CompletionPortThreads != null))
        {
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
            ThreadPool.SetMinThreads(MinThreads.WorkerThreads ?? minWorkerThreads,
                MinThreads.CompletionPortThreads ?? minCompletionPortThreads);
        }

        if (MaxThreads != null && (MaxThreads.WorkerThreads != null || MaxThreads.CompletionPortThreads != null))
        {
            ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
            ThreadPool.SetMaxThreads(MaxThreads.WorkerThreads ?? maxWorkerThreads,
                MaxThreads.CompletionPortThreads ?? maxCompletionPortThreads);
        }
    }
}