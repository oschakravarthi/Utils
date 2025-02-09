using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace SubhadraSolutions.Utils;

[Pure]
public sealed class OrderComparer<T> : IComparer<T> where T : MemberInfo
{
    private OrderComparer()
    {
    }

    public static OrderComparer<T> Instance { get; } = new();

    public int Compare(T x, T y)
    {
        var xOrder = GetOrder(x);
        var yOrder = GetOrder(y);
        if (xOrder == yOrder)
        {
            return 0;
        }

        if (xOrder == null && yOrder != null)
        {
            return 1;
        }

        if (xOrder != null && yOrder == null)
        {
            return -1;
        }

        return xOrder.Value.CompareTo(yOrder.Value);
    }

    public static List<T> FilterMembersWithOrderAttribute(IEnumerable<T> members)
    {
        var list = new List<T>();

        foreach (var member in members)
        {
            Attribute attribute = member.GetCustomAttribute<DisplayAttribute>(false);
            if (attribute == null)
            {
                attribute = member.GetCustomAttribute<ColumnAttribute>(false);
            }

            if (attribute != null)
            {
                list.Add(member);
            }
        }

        return list;
    }

    public static void RemoveMembersWithoutOrderAttribute(IList<T> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            Attribute attribute = list[i].GetCustomAttribute<DisplayAttribute>(false);
            if (attribute == null)
            {
                attribute = list[i].GetCustomAttribute<ColumnAttribute>(false);
            }

            if (attribute == null)
            {
                list.RemoveAt(i);
                i--;
            }
        }
    }

    private static int? GetOrder(T member)
    {
        var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(member, typeof(DisplayAttribute));

        if (displayAttribute != null)
        {
            return displayAttribute.GetOrder();
        }

        var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(member, typeof(ColumnAttribute));

        if (columnAttribute != null)
        {
            return columnAttribute.Order;
        }

        return null;
    }
}