using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public interface ISimpleQueue<T>
{
    IEnumerable<T> Dequeue();

    void Enqueue(T obj);

    void Enqueue(IEnumerable<T> objects);
}