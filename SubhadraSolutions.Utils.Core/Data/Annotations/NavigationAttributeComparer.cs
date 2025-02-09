using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils.Data.Annotations;

[Pure]
public sealed class NavigationAttributeComparer : IComparer<NavigationAttribute>
{
    public static NavigationAttributeComparer Instance { get; } = new();

    public int Compare(NavigationAttribute x, NavigationAttribute y)
    {
        if (x.LinkTemplate == null && y.LinkTemplate == null)
        {
            return 0;
        }

        if (x.LinkTemplate == null)
        {
            return -1;
        }

        if (y.LinkTemplate == null)
        {
            return 1;
        }

        var isXRooted = x.LinkTemplate.StartsWith("http", StringComparison.OrdinalIgnoreCase);
        var isYRooted = y.LinkTemplate.StartsWith("http", StringComparison.OrdinalIgnoreCase);
        if (isXRooted && !isYRooted)
        {
            return 1;
        }

        if (!isXRooted && isYRooted)
        {
            return -1;
        }

        var result = string.Compare(x.Name, y.Name);
        if (result != 0)
        {
            return result;
        }

        return string.Compare(x.Target, y.Target, true);
    }
}