using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class SequencedDuplicatedRemoverEnumerableDecorator<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> actual;
    private readonly Func<T, T, bool> equalityComparerFunc;

    public SequencedDuplicatedRemoverEnumerableDecorator(IEnumerable<T> actual,
        Func<T, T, bool> equalityComparerFunc)
    {
        this.actual = actual;
        this.equalityComparerFunc = equalityComparerFunc;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new SequencedDuplicatedRemoverEnumeratorDecorator<T>(actual.GetEnumerator(), equalityComparerFunc);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}