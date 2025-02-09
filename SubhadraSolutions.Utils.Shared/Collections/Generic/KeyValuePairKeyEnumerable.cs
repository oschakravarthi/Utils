using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class KeyValuePairKeyEnumerable<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairs) : IEnumerable<TKey>
{
    public IEnumerator<TKey> GetEnumerator()
    {
        return new KeyValuePairKeyEnumerator<TKey, TValue>(pairs.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}