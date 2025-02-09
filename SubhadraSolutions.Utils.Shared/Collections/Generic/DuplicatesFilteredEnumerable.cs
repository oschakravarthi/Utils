using SubhadraSolutions.Utils.Validation;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public sealed class DuplicatesFilteredEnumerable<T> : IEnumerable<T>
{
    private readonly IEqualityComparer<T> _comparer;
    private readonly IEnumerable<T> adaptedObject;

    public DuplicatesFilteredEnumerable(IEnumerable<T> adaptedObject)
    {
        Guard.ArgumentShouldNotBeNull(adaptedObject, nameof(adaptedObject));
        this.adaptedObject = adaptedObject;
    }

    public DuplicatesFilteredEnumerable(IEnumerable<T> adaptedObject, IEqualityComparer<T> comparer)
        : this(adaptedObject)
    {
        Guard.ArgumentShouldNotBeNull(comparer, nameof(comparer));
        _comparer = comparer;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_comparer == null)
        {
            return new DuplicatesFilteredEnumerator<T>(adaptedObject.GetEnumerator());
        }

        return new DuplicatesFilteredEnumerator<T>(adaptedObject.GetEnumerator(), _comparer);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}