using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class SequencedDuplicatedRemoverEnumeratorDecorator<T> : AbstractDisposable, IEnumerator<T>
{
    private readonly IEnumerator<T> actual;
    private readonly Func<T, T, bool> equalityComparerFunc;
    private bool isFirst = true;
    private T previous;

    public SequencedDuplicatedRemoverEnumeratorDecorator(IEnumerator<T> actual,
        Func<T, T, bool> equalityComparerFunc)
    {
        this.actual = actual;
        this.equalityComparerFunc = equalityComparerFunc;
    }

    public T Current { get; private set; }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        var canMove = actual.MoveNext();
        if (!canMove)
        {
            return false;
        }

        Current = actual.Current;
        if (isFirst)
        {
            previous = Current;
            isFirst = false;
            return true;
        }

        while (equalityComparerFunc(Current, previous))
        {
            if (!actual.MoveNext())
            {
                return false;
            }

            Current = actual.Current;
        }

        previous = Current;
        return true;
    }

    public void Reset()
    {
        isFirst = true;
        actual.Reset();
    }

    protected override void Dispose(bool disposing)
    {
        actual.Dispose();
    }
}