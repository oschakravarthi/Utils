using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class CounterEnumeratorDecorator<T>(IEnumerator<T> enumerator) : AbstractDisposable, IEnumerator<T>
{
    public int ReadCount { get; private set; }

    public T Current
    {
        get
        {
            ReadCount++;
            return enumerator.Current;
        }
    }

    object IEnumerator.Current
    {
        get
        {
            ReadCount++;
            return enumerator.Current;
        }
    }

    public bool MoveNext()
    {
        return enumerator.MoveNext();
    }

    public void Reset()
    {
        enumerator.Reset();
    }

    protected override void Dispose(bool disposing)
    {
        enumerator.Dispose();
    }
}