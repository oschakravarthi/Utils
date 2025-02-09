using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace SubhadraSolutions.Utils.Reflection;

public static class TypesLookupHelper
{
    private static readonly ConcurrentDictionary<string, Type> typesDictionary = new();

    static TypesLookupHelper()
    {
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            PopulateFromAssembly(assembly);
        }
    }

    public static Type GetType(string typeName)
    {
        typesDictionary.TryGetValue(typeName, out var type);
        return type;
    }

    private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        PopulateFromAssembly(args.LoadedAssembly);
    }

    private static void PopulateFromAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            typesDictionary.TryAdd(type.FullName, type);
        }
    }
}