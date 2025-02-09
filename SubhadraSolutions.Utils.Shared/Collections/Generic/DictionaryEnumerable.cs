using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class DictionaryEnumerable<TKey, TValue>(IDictionary<TKey, TValue> dictionary, EnterOrExitReadLock callback)
    : IEnumerable<KeyValuePair<TKey, TValue>>
{
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new DictionaryEnumerator<TKey, TValue>(dictionary, callback);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}