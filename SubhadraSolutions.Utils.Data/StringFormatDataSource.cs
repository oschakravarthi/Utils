using SubhadraSolutions.Utils.Threading;

namespace SubhadraSolutions.Utils.Data;

public class StringFormatDataSource : IDataSource<string>
{
    public IDataSource<object>[] DataSources { get; set; }
    public int NumberOfThreads { get; set; }
    public IDataSource<string> TemplateSource { get; set; }

    public string GetData()
    {
        var template = TemplateSource.GetData();
        var count = DataSources.Length;
        var parts = new object[count];

        if (NumberOfThreads < 2)
        {
            for (var i = 0; i < count; i++) parts[i] = DataSources[i].GetData();
        }
        else
        {
            var parallelExecutor = new ParallelExecutor(NumberOfThreads, false);
            for (var i = 0; i < count; i++)
            {
                var j = i;
                parallelExecutor.AddAction(new ActionInfo(() =>
                {
                    var content = DataSources[j].GetData();
                    parts[j] = content;
                }, 0));
            }

            parallelExecutor.ExecuteAll();
        }

        return string.Format(template, parts);
    }
}