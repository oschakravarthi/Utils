using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class ListAdapter<T> : IDataStructureAdapter<T>
{
    protected int _capacityInt;
    protected IList<T> adaptedObject;

    public ListAdapter(IList<T> adaptedObject)
    {
        this.adaptedObject = adaptedObject;
        _capacityInt = this.adaptedObject.Count;
    }

    public ListAdapter(IList<T> adaptedObject, int capacity)
    {
        this.adaptedObject = adaptedObject;
        _capacityInt = capacity;
    }

    public int Count => adaptedObject.Count;

    public virtual int Capacity => _capacityInt;

    public T this[int index]
    {
        get => adaptedObject.Count > index ? adaptedObject[index] : default;

        set
        {
            if (adaptedObject.Count > index)
            {
                adaptedObject[index] = value;
            }
        }
    }

    public void Clear()
    {
        adaptedObject.Clear();
    }
}