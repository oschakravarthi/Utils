using SubhadraSolutions.Utils.Threading;

namespace SubhadraSolutions.Utils.Data;

public class CompositeDataSource<T> : IDataSource<T[]>
{
    public IDataSource<T>[] DataSources { get; set; }
    public int NumberOfThreads { get; set; }

    public T[] GetData()
    {
        var count = DataSources.Length;
        var result = new T[count];

        if (NumberOfThreads < 2)
        {
            for (var i = 0; i < count; i++) result[i] = DataSources[i].GetData();
            return result;
        }

        var parallelExecutor = new ParallelExecutor(NumberOfThreads, false);
        for (var i = 0; i < count; i++)
        {
            var j = i;
            parallelExecutor.AddAction(new ActionInfo(() =>
            {
                var content = DataSources[j].GetData();
                result[j] = content;
            }, 0));
        }

        parallelExecutor.ExecuteAll();
        return result;
    }
}