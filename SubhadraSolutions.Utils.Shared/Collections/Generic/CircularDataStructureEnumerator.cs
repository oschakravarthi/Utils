using System;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class CircularDataStructureEnumerator<T> : DataStructureEnumerator<T>
    where T : class
{
    protected IDataStructureAdapter<T> circularDataStructure;

    private bool inCurrentIndexCrossed;

    protected int lastWrittenIndex;

    public CircularDataStructureEnumerator(IDataStructureAdapter<T> store, int startIndex, int lastWrittenIndex,
        bool isCycleCompleted)
        : base(store, startIndex, lastWrittenIndex)
    {
        circularDataStructure = store;
        this.lastWrittenIndex = lastWrittenIndex;
        _indexInt = isCycleCompleted ? this.lastWrittenIndex - 1 : startIndex - 1;
    }

    public override bool MoveNext()
    {
        if ((_indexInt == -1 && lastWrittenIndex == -1) || inCurrentIndexCrossed)
        {
            return false;
        }

        _indexInt = (_indexInt + 1) % _dataStructure.Capacity;
        if (_indexInt == lastWrittenIndex && !inCurrentIndexCrossed)
        {
            inCurrentIndexCrossed = true;
        }

        return true;
    }
}