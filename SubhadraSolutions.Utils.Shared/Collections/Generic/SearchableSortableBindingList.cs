using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class SearchableSortableBindingList<T> : BindingList<T>
{
    private T _current;

    private bool _isSorted;

    public T Current
    {
        get => _current;

        set
        {
            if (Contains(value))
            {
                _current = value;
            }
            else
            {
                throw new InvalidDataException("This item does n't exist in the collection");
            }
        }
    }

    public ListSortDirection SortDirection { get; private set; }

    public PropertyDescriptor SortProperty { get; private set; }

    protected override bool IsSortedCore => _isSorted;

    protected override ListSortDirection SortDirectionCore => SortDirection;

    protected override PropertyDescriptor SortPropertyCore => SortProperty;

    protected override bool SupportsSearchingCore => true;

    protected override bool SupportsSortingCore => true;

    public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
    {
        ApplySortCore(property, direction);
    }

    public void Load(IList<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
    {
        if (Items is List<T> items)
        {
            var comparer = DynamicComparerHelper<T>.GetComparerForProperty(property.Name);
            if (comparer == null)
            {
                return;
            }

            if (direction == ListSortDirection.Descending)
            {
                comparer = new InverseComparer<T>(comparer);
            }

            items.Sort(comparer);
            _isSorted = true;
        }
        else
        {
            _isSorted = false;
        }

        SortProperty = property;
        SortDirection = direction;

        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override int FindCore(PropertyDescriptor property, object key)
    {
        if (property == null)
        {
            return -1;
        }

        var items = Items;
        foreach (var item in items)
        {
            var value = (string)property.GetValue(item);
            if ((string)key == value)
            {
                return IndexOf(item);
            }
        }

        return -1;
    }

    protected override void RemoveSortCore()
    {
        _isSorted = false;
        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }
}