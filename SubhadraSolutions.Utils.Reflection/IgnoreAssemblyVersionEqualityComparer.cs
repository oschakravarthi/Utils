using SubhadraSolutions.Utils.Abstractions;
using System.Diagnostics.Contracts;
using System.IO;

namespace SubhadraSolutions.Utils.Reflection;

[Pure]
public sealed class IgnoreAssemblyVersionEqualityComparer : AbstractEqualityComparer<string>
{
    private IgnoreAssemblyVersionEqualityComparer()
    {
    }

    public static IgnoreAssemblyVersionEqualityComparer Instance { get; } = new();

    public override int GetHashCode(string obj)
    {
        var parser = new TypeParser(obj);
        return (parser.ShortAssemblyName + Path.PathSeparator + parser.ShortTypeName).GetHashCode();
    }

    protected override bool EqualsProtected(string x, string y)
    {
        if (x == y)
        {
            return true;
        }

        var parserX = new TypeParser(x);
        var parserY = new TypeParser(y);
        if (parserX.ShortAssemblyName == parserY.ShortAssemblyName)
        {
            if (parserX.ShortTypeName == parserY.ShortTypeName)
            {
                return true;
            }
        }

        return false;
    }
}