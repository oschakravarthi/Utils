using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class SlidingWindow<T>(long size)
{
    private readonly T[] _array = new T[size];

    private bool _isCycleCompleted;

    private long _lastWrittenIndex = -1;

    public long ItemsCount => _isCycleCompleted ? _array.LongLength : _lastWrittenIndex;

    public void Add(T item)
    {
        var index = (_lastWrittenIndex + 1) % _array.LongLength;
        _array[index] = item;
        if (index < _lastWrittenIndex)
        {
            _isCycleCompleted = true;
        }

        _lastWrittenIndex = index;
    }

    public IEnumerable<T> GetAllItems()
    {
        var startIndex = _isCycleCompleted ? _lastWrittenIndex + 1 : 0;
        var endIndex = _isCycleCompleted ? _lastWrittenIndex + _array.LongLength : _lastWrittenIndex;
        var index = startIndex;
        for (var i = index; i <= endIndex; i++) yield return _array[i % _array.LongLength];
    }

    public bool TryGetLast(out T item)
    {
        item = default;
        if (_lastWrittenIndex > -1)
        {
            item = _array[_lastWrittenIndex];
            return true;
        }

        return false;
    }
}