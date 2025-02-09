using System;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Threading.Tasks;

public class ItemTaskContext<T>
{
    public ItemTaskContext(T item, Task task)
    {
        Item = item;
        Task = task;
    }

    public T Item { get; private set; }
    public Exception Exception { get; set; }
    public Task Task { get; private set; }
}