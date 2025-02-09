using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class KeyValuePairKeyEnumerator<TKey, TValue>
    (IEnumerator<KeyValuePair<TKey, TValue>> enumerator) : AbstractDisposable, IEnumerator<TKey>
{
    public TKey Current => enumerator.Current.Key;

    object IEnumerator.Current => enumerator.Current.Key;

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