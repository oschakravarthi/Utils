using SubhadraSolutions.Utils.Data.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class AttributeLookupSupportedColumnOrderComparer(AttributesLookup attributesLookup) : IComparer<PropertyInfo>
{
    public int Compare(PropertyInfo x, PropertyInfo y)
    {
        var xOrder = GetOrder(x);
        var yOrder = GetOrder(y);
        return xOrder.CompareTo(yOrder);
    }

    private int GetOrder(PropertyInfo property)
    {
        var displayAttribute = attributesLookup.GetCustomAttribute<DisplayAttribute>(property);

        if (displayAttribute != null)
        {
            return displayAttribute.Order;
        }

        var columnAttribute = attributesLookup.GetCustomAttribute<ColumnAttribute>(property);

        if (columnAttribute != null)
        {
            return columnAttribute.Order;
        }

        return int.MaxValue;
    }
}