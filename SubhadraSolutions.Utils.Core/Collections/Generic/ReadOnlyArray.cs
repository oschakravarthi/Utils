using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class ReadOnlyArray<T> : IEnumerable<T>
{
    private readonly T[] _array;

    public ReadOnlyArray(T[] array)
    {
        this._array = array;
    }

    public int Length => this._array.Length;
    public long LongLength => this._array.LongLength;

    public T this[int index] => this._array[index];

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)this._array).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}