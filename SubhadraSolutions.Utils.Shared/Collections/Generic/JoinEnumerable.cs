using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public sealed class JoinEnumerable<T>(IEnumerable<IEnumerable<T>> enumerables) : IEnumerable<T>
{
    private readonly IEnumerable<IEnumerable<T>> _enumerables = enumerables;

    public IEnumerator<T> GetEnumerator()
    {
        return new JoinEnumerator<T>(BuildEnumerators());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private List<IEnumerator<T>> BuildEnumerators()
    {
        var enumerators = new List<IEnumerator<T>>();
        foreach (var enumerable in _enumerables)
        {
            var enumerator = enumerable.GetEnumerator();
            enumerators.Add(enumerator);
        }

        return enumerators;
    }
}