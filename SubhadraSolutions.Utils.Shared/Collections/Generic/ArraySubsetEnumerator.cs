using SubhadraSolutions.Utils.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class ArraySubsetEnumerator<T>(T[] array, int count) : AbstractDisposable, IEnumerator<T>
{
    private int current = -1;

    public T Current
    {
        get
        {
            if (current == -1)
            {
                return default;
            }

            return array[current];
        }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (current < count - 1)
        {
            current++;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        current = -1;
    }

    protected override void Dispose(bool disposing)
    {
        //There is nothing to dispose
    }
}