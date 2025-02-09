using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class KeyValuePairValueEnumerable<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairs) : IEnumerable<TValue>
{
    public IEnumerator<TValue> GetEnumerator()
    {
        return new KeyValuePairValueEnumerator<TKey, TValue>(pairs.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}