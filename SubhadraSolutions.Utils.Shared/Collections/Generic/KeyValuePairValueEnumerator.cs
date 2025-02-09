using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class KeyValuePairValueEnumerator<TKey, TValue>
    (IEnumerator<KeyValuePair<TKey, TValue>> enumerator) : AbstractDisposable, IEnumerator<TValue>
{
    public TValue Current => enumerator.Current.Value;

    object IEnumerator.Current => enumerator.Current.Value;

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