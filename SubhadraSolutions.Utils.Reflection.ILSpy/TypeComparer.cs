using ICSharpCode.Decompiler.TypeSystem;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SubhadraSolutions.Utils.Reflection.ILSpy;

[Pure]
public sealed class TypeComparer : IComparer<IType>
{
    public static readonly TypeComparer Instance = new();

    private TypeComparer()
    {
    }

    public int Compare(IType x, IType y)
    {
        if (x.DeclaringType == y)
        {
            return 1;
        }

        if (y.DeclaringType == x)
        {
            return -1;
        }

        if (x.DeclaringType != null && y.DeclaringType == null)
        {
            return 1;
        }

        if (x.DeclaringType == null && y.DeclaringType != null)
        {
            return -1;
        }

        var t = x.DeclaringType;
        while (t != null)
        {
            if (t == y)
            {
                return 1;
            }

            t = t.DeclaringType;
        }

        t = y.DeclaringType;
        while (t != null)
        {
            if (t == x)
            {
                return -1;
            }

            t = t.DeclaringType;
        }

        return 0;
    }
}