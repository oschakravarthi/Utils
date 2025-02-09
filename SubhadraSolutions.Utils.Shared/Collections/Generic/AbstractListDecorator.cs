using SubhadraSolutions.Utils.Validation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public abstract class AbstractListDecorator<T> : AbstractCollectionDecorator<T>, IList<T>, IList
{
    protected AbstractListDecorator(IList<T> adaptedList)
        : base(adaptedList)
    {
        Guard.ArgumentShouldNotBeNull(adaptedList, nameof(adaptedList));
        this.AdaptedList = adaptedList;
    }

    protected IList<T> AdaptedList { get; }

    int ICollection.Count => Count;

    bool IList.IsFixedSize => AdaptedList is Array;

    bool IList.IsReadOnly => IsReadOnly;

    bool ICollection.IsSynchronized
    {
        get
        {
            if (AdaptedList is IList list)
            {
                return list.IsSynchronized;
            }

            return false;
        }
    }

    object ICollection.SyncRoot
    {
        get
        {
            if (AdaptedList is IList list)
            {
                return list.SyncRoot;
            }

            return null;
        }
    }

    object IList.this[int index]
    {
        get => this[index];

        set => this[index] = (T)value;
    }

    int IList.Add(object value)
    {
        Add((T)value);
        return Count - 1;
    }

    void IList.Clear()
    {
        Clear();
    }

    bool IList.Contains(object value)
    {
        return Contains((T)value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        CopyTo((T[])array, index);
    }

    int IList.IndexOf(object value)
    {
        return IndexOf((T)value);
    }

    void IList.Insert(int index, object value)
    {
        Insert(index, (T)value);
    }

    void IList.Remove(object value)
    {
        Remove((T)value);
    }

    void IList.RemoveAt(int index)
    {
        RemoveAt(index);
    }

    public virtual T this[int index]
    {
        get => AdaptedList[index];

        set => AdaptedList[index] = value;
    }

    public virtual int IndexOf(T item)
    {
        return AdaptedList.IndexOf(item);
    }

    public virtual void Insert(int index, T item)
    {
        AdaptedList.Insert(index, item);
    }

    public virtual void RemoveAt(int index)
    {
        AdaptedList.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}