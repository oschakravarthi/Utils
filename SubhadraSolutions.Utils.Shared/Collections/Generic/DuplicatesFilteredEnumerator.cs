using SubhadraSolutions.Utils.Abstractions;
using SubhadraSolutions.Utils.Validation;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class DuplicatesFilteredEnumerator<T> : AbstractDisposable, IEnumerator<T>
{
    private readonly HashSet<T> _hashSet;
    private readonly IEnumerator<T> adaptedObject;

    public DuplicatesFilteredEnumerator(IEnumerator<T> adaptedObject)
    {
        Guard.ArgumentShouldNotBeNull(adaptedObject, nameof(adaptedObject));
        this.adaptedObject = adaptedObject;
        _hashSet = [];
    }

    public DuplicatesFilteredEnumerator(IEnumerator<T> adaptedObject, IEqualityComparer<T> comparer)
    {
        Guard.ArgumentShouldNotBeNull(adaptedObject, nameof(adaptedObject));
        Guard.ArgumentShouldNotBeNull(comparer, nameof(comparer));
        this.adaptedObject = adaptedObject;
        _hashSet = new HashSet<T>(comparer);
    }

    public T Current => adaptedObject.Current;

    object IEnumerator.Current => adaptedObject.Current;

    public bool MoveNext()
    {
        while (adaptedObject.MoveNext())
            if (_hashSet.Add(adaptedObject.Current))
            {
                return true;
            }

        return false;
    }

    public void Reset()
    {
        adaptedObject.Reset();
        _hashSet.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        adaptedObject.Dispose();
        _hashSet.Clear();
    }
}