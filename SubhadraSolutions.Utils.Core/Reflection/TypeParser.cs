using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SubhadraSolutions.Utils.Reflection;

public class TypeParser
{
    private static readonly Lazy<Assembly[]> Assemblies = new(AppDomain.CurrentDomain.GetAssemblies);
    public readonly string AssemblyDescriptionString;
    public readonly string Culture;
    public readonly string FullyQualifiedTypeName;
    public readonly List<TypeParser> GenericParameters = [];
    public readonly string PublicKeyToken;
    public readonly string ShortAssemblyName;
    public readonly string ShortTypeName;
    public readonly string Version;

    public TypeParser(string assemblyQualifiedName)
    {
        FullyQualifiedTypeName = assemblyQualifiedName;
        var index = -1;
        var rootBlock = new Block();
        {
            var bcount = 0;
            var currentBlock = rootBlock;
            for (var i = 0; i < assemblyQualifiedName.Length; ++i)
            {
                var c = assemblyQualifiedName[i];
                if (c == '[')
                {
                    ++bcount;
                    var b = new Block { iStart = i + 1, level = bcount, parentBlock = currentBlock };
                    currentBlock.innerBlocks.Add(b);
                    currentBlock = b;
                }
                else if (c == ']')
                {
                    currentBlock.iEnd = i - 1;
                    if (assemblyQualifiedName[currentBlock.iStart] != '[')
                    {
                        currentBlock.parsedAssemblyQualifiedName = new TypeParser(
                            assemblyQualifiedName.Substring(currentBlock.iStart, i - currentBlock.iStart));
                        if (bcount == 2)
                        {
                            GenericParameters.Add(currentBlock.parsedAssemblyQualifiedName);
                        }
                    }

                    currentBlock = currentBlock.parentBlock;
                    --bcount;
                }
                else if (bcount == 0 && c == ',')
                {
                    index = i;
                    break;
                }
            }
        }

        ShortTypeName = assemblyQualifiedName.Substring(0, index);

        AssemblyDescriptionString = assemblyQualifiedName.Substring(index + 2);
        {
            var parts = AssemblyDescriptionString.Split(',').Select(x => x.Trim()).ToList();
            Version = LookForPairThenRemove(parts, "Version");
            Culture = LookForPairThenRemove(parts, "Culture");
            PublicKeyToken = LookForPairThenRemove(parts, "PublicKeyToken");
            if (parts.Count > 0)
            {
                ShortAssemblyName = parts[0];
            }
        }
    }

    internal string LanguageStyle(string prefix, string suffix)
    {
        if (GenericParameters.Count > 0)
        {
            var sb = new StringBuilder(ShortTypeName.Substring(0, ShortTypeName.IndexOf('`')));
            sb.Append(prefix);
            var pendingElement = false;
            foreach (var param in GenericParameters)
            {
                if (pendingElement)
                {
                    sb.Append(", ");
                }

                sb.Append(param.LanguageStyle(prefix, suffix));
                pendingElement = true;
            }

            sb.Append(suffix);
            return sb.ToString();
        }

        return ShortTypeName;
    }

    private static string LookForPairThenRemove(IList<string> strings, string name)
    {
        for (var istr = 0; istr < strings.Count; istr++)
        {
            var s = strings[istr];
            var i = s.IndexOf(name, StringComparison.Ordinal);
            if (i == 0)
            {
                var i2 = s.IndexOf('=');
                if (i2 > 0)
                {
                    var ret = s.Substring(i2 + 1);
                    strings.RemoveAt(istr);
                    return ret;
                }
            }
        }

        return null;
    }

    private sealed class Block
    {
        internal readonly List<Block> innerBlocks = [];

        internal int iEnd;

        internal int iStart;

        internal int level;

        internal Block parentBlock;

        internal TypeParser parsedAssemblyQualifiedName;
    }
}