using SubhadraSolutions.Utils.Validation;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public abstract class AbstractCollectionDecorator<T> : ICollection<T>
{
    protected AbstractCollectionDecorator(ICollection<T> adaptedCollection)
    {
        Guard.ArgumentShouldNotBeNull(adaptedCollection, nameof(adaptedCollection));
        this.AdaptedCollection = adaptedCollection;
    }

    protected ICollection<T> AdaptedCollection { get; }

    public virtual int Count => AdaptedCollection.Count;

    public virtual bool IsReadOnly => AdaptedCollection.IsReadOnly;

    public virtual void Add(T item)
    {
        AdaptedCollection.Add(item);
    }

    public virtual void Clear()
    {
        AdaptedCollection.Clear();
    }

    public virtual bool Contains(T item)
    {
        return AdaptedCollection.Contains(item);
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        AdaptedCollection.CopyTo(array, arrayIndex);
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
        return AdaptedCollection.GetEnumerator();
    }

    public virtual bool Remove(T item)
    {
        return AdaptedCollection.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}