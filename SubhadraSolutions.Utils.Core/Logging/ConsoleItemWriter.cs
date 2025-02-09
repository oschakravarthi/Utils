using SubhadraSolutions.Utils.Exposition;
using System;

namespace SubhadraSolutions.Utils.Logging;

public class ConsoleItemWriter<T> : IItemWriter<T>
{
    [Expose(httpRequestMethod: HttpRequestMethod.Post)]
    public void Write(T item)
    {
        Console.WriteLine(item);
    }
}