using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class NavigationAttribute : Attribute, IComparable<NavigationAttribute>,
    IEquatable<NavigationAttribute>
{
    public NavigationAttribute(string linkTemplate, bool encode = false, string target = null, string name = null)
    {
        LinkTemplate = linkTemplate;
        Encode = encode;
        Target = target;
        Name = name;
    }

    public bool Encode { get; }
    public string LinkTemplate { get; }
    public string Name { get; }
    public string Target { get; }

    public int CompareTo(NavigationAttribute other)
    {
        return string.Compare(LinkTemplate, other.LinkTemplate, StringComparison.CurrentCultureIgnoreCase);
    }

    public bool Equals(NavigationAttribute other)
    {
        return CompareTo(other) == 0;
    }

    public static NavigationAttribute GetSuitableAttribute(IEnumerable<NavigationAttribute> attributes,
        bool external)
    {
        if (attributes == null)
        {
            return null;
        }

        foreach (var nav in attributes)
        {
            if (nav.LinkTemplate == null)
            {
                continue;
            }

            var isExternalLink = nav.LinkTemplate.StartsWith("http", StringComparison.CurrentCultureIgnoreCase);

            if (external && isExternalLink)
            {
                return nav;
            }

            if (!external && !isExternalLink)
            {
                return nav;
            }
        }

        return null;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as NavigationAttribute);
    }

    public override int GetHashCode()
    {
        return LinkTemplate == null ? 0 : LinkTemplate.ToUpper().GetHashCode();
    }
}