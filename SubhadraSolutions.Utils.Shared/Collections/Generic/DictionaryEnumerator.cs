using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public delegate void EnterOrExitReadLock(bool isEnter);

public sealed class DictionaryEnumerator<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
        EnterOrExitReadLock callback)
    : AbstractDisposable, IEnumerator<KeyValuePair<TKey, TValue>>
{
    private IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

    public KeyValuePair<TKey, TValue> Current => _enumerator.Current;

    object IEnumerator.Current => _enumerator.Current;

    public bool MoveNext()
    {
        if (_enumerator == null)
        {
            callback(true);
        }

        _enumerator = dictionary.GetEnumerator();
        return _enumerator.MoveNext();
    }

    public void Reset()
    {
        _enumerator = null;
    }

    protected override void Dispose(bool disposing)
    {
        callback(false);
        _enumerator = null;
    }
}