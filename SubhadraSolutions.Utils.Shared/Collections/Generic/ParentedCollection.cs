using Newtonsoft.Json;
using SubhadraSolutions.Utils.Contracts;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class ParentedCollection<T, P> : ICollection<T> where T : IParented<P>
{
    private readonly List<T> list = [];
    private readonly P parent;

    public ParentedCollection(P parent)
    {
        this.parent = parent;
    }

    [JsonConstructor]
    [System.Text.Json.Serialization.JsonConstructor]
    private ParentedCollection()
    {
    }

    public T this[int index] => list[index];

    public int Count => list.Count;

    //[JsonIgnore]
    public bool IsReadOnly => false;

    public void Add(T item)
    {
        item.Parent = parent;
        list.Add(item);
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public bool Remove(T item)
    {
        return list.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public int IndexOf(T item)
    {
        return list.IndexOf(item);
    }
}