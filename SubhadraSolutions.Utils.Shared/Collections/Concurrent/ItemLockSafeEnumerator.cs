using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SubhadraSolutions.Utils.Collections.Concurrent;

public sealed class ItemLockSafeEnumerator<T> : AbstractDisposable, IEnumerator<T> where T : class
{
    private readonly T previous = null;
    private readonly IEnumerator<T> enumerator;

    public ItemLockSafeEnumerator(IEnumerator<T> enumerator)
    {
        this.enumerator = enumerator;
    }

    object IEnumerator.Current => enumerator.Current;

    T IEnumerator<T>.Current => enumerator.Current;

    public bool MoveNext()
    {
        var canMove = enumerator.MoveNext();
        if (previous != null)
        {
            Monitor.Exit(previous);
        }

        if (canMove)
        {
            Monitor.Enter(enumerator.Current);
        }

        return canMove;
    }

    public void Reset()
    {
        enumerator.Reset();
    }

    ~ItemLockSafeEnumerator()
    {
        Dispose(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (enumerator.Current != null)
        {
            Monitor.Exit(enumerator.Current);
        }

        enumerator.Dispose();
    }
}