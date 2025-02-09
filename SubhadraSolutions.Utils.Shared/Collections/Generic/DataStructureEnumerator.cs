using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class DataStructureEnumerator<T>(IDataStructureAdapter<T> dataStructure, int startIndex, int endIndex)
    : AbstractDisposable, IKnownCountEnumerator<T>
{
    protected IDataStructureAdapter<T> _dataStructure = dataStructure;

    protected int _endIndex = endIndex;

    protected int _indexInt = -1;

    protected int _startIndexInt = startIndex;

    public int Count => _endIndex - _startIndexInt;

    public T Current => _dataStructure[_indexInt];

    object IEnumerator.Current => _dataStructure[_indexInt];

    public virtual bool MoveNext()
    {
        _indexInt++;
        return _indexInt <= _endIndex; // && _indexInt < _dataStructure.Count;
    }

    public void Reset()
    {
        _indexInt = _startIndexInt - 1;
    }

    protected override void Dispose(bool disposing)
    {
    }
}