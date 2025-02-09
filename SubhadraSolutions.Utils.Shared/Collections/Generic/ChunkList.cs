using SubhadraSolutions.Utils.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

[Serializable]
public class ChunkList<T>(int chunkSize) : ChunkArray<T>(chunkSize, chunkSize), IList<T>
{
    protected int _itemsCount;

    public ChunkList()
        : this(256)
    {
    }

    public ChunkList(IEnumerable<T> elements)
        : this()
    {
        Add(elements);
    }

    public int Count => _itemsCount;

    public bool IsReadOnly => false;

    public void Add(T element)
    {
        ResizeIfRequired();
        base[_itemsCount] = element;
        _itemsCount++;
    }

    public override void Clear()
    {
        capacity = chunkSize;
        InitializeProtected();
        _itemsCount = 0;
    }

    public bool Contains(T item)
    {
        var index = IndexOf(item);
        return index > -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < _itemsCount; i++) array[i + arrayIndex] = base[i];
    }

    public override IEnumerator<T> GetEnumerator()
    {
        return new DataStructureEnumerator<T>(this, 0, _itemsCount);
    }

    public int IndexOf(T item)
    {
        for (var i = 0; i < _itemsCount; i++)
            if (EqualityComparer<T>.Default.Equals(base[i], item))
            {
                return i;
            }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index >= _itemsCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        ResizeIfRequired();
        _itemsCount++;
        for (var i = _itemsCount - 1; i >= index; i--) base[i + 1] = base[i];

        base[index] = item;
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index > -1)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _itemsCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        base[index] = DefaultValue;
        for (var i = index + 1; i < _itemsCount; i++) base[i - 1] = base[i];

        _itemsCount--;
        if (_itemsCount >= chunkSize && _itemsCount % chunkSize == 0)
        {
            ChunkArrays.RemoveAt(ChunkArrays.Count - 1);
        }
        else
        {
            base[_itemsCount] = DefaultValue;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static ChunkList<T> Create(IEnumerable<T> items)
    {
        var a = new ChunkList<T> { items };
        return a;
    }

    public void Add(IEnumerable<T> elements)
    {
        foreach (var element in elements)
        {
            Add(element);
        }
    }

    public void Sort()
    {
        this.MergeSort();
    }

    public void Sort(IComparer<T> comparer)
    {
        this.MergeSort(comparer);
    }

    public void Sort(Comparison<T> comparison)
    {
        this.MergeSort(comparison);
    }

    private void ResizeIfRequired()
    {
        if (_itemsCount >= capacity)
        {
            ChunkArrays.Add(new T[chunkSize]);
            capacity += chunkSize;
        }
    }
}