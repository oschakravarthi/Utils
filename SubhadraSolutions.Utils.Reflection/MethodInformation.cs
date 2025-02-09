using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

public delegate TReturnType DynamicMethodDeligate<out TReturnType, in TParameterType>(TParameterType obj);

[Serializable]
public sealed class MethodInformation : AbstractMethodInformation
{
    internal MethodInformation()
    {
        Instructions = [];
    }

    internal List<MsilInstruction> Instructions { get; set; }

    internal string[] LocalVariableTypes { get; set; }

    internal string ReturnType { get; set; }

    public static MethodInformation BuildMethodInformation(MethodInfo method)
    {
        return MsilReader.BuildMethodInformation(method);
    }

    public void ApplyTypeMappings(string serverType, string clientType, bool ignoreAssemblyVersion)
    {
        var mappings = ignoreAssemblyVersion
            ? new Dictionary<string, string>(IgnoreAssemblyVersionEqualityComparer.Instance)
            : [];
        mappings.Add(serverType, clientType);
        ApplyTypeMappings(mappings);
    }

    public void ApplyTypeMappings(IDictionary<string, string> mappings)
    {
        var splits = splitTypeNames(ReturnType);
        ReturnType = applyMapping(ReturnType, splits, mappings);
        applyMapping(ParameterTypes, mappings);
        applyMapping(LocalVariableTypes, mappings);
        foreach (var ins in Instructions)
        {
            if (ins.TypeName != null)
            {
                splits = splitTypeNames(ins.TypeName);
                ins.TypeName = applyMapping(ins.TypeName, splits, mappings);
            }

            if (ins.MethodCallInfo != null)
            {
                applyMapping(ins.MethodCallInfo.ParameterTypes, mappings);
            }
        }
    }

    public DynamicMethod BuildDynamicMethod()
    {
        return MsilWriter.BuildDynamicMethod(this);
    }

    public DynamicMethodDeligate<TReturnType, TParameterType> BuildMethodDeligate<TReturnType, TParameterType>()
    {
        var method = MsilWriter.BuildDynamicMethod(this);
        var del = (DynamicMethodDeligate<TReturnType, TParameterType>)method.CreateDelegate(
            typeof(DynamicMethodDeligate<TReturnType, TParameterType>));
        return del;
    }

    private static void applyMapping(IList<string> serverTypes, IDictionary<string, string> mappings)
    {
        for (var i = 0; i < serverTypes.Count; i++)
        {
            var splits = splitTypeNames(serverTypes[i]);
            serverTypes[i] = applyMapping(serverTypes[i], splits, mappings);
        }
    }

    private static string applyMapping(string mainString, IEnumerable<string> children,
        IDictionary<string, string> mappings)
    {
        foreach (var v in children)
        {
            var mappingString = getMapping(v, mappings);
            mainString = mainString.Replace(v, mappingString);
        }

        return mainString;
    }

    private static string getMapping(string serverTypeName, IDictionary<string, string> mappings)
    {
        if (!mappings.TryGetValue(serverTypeName, out var clientTypeName))
        {
            clientTypeName = serverTypeName;
        }

        return clientTypeName;
    }

    private static List<string> splitTypeNames(string typeName)
    {
        var list = new List<string>();
        var parser = new TypeParser(typeName);

        splitTypeNamesRecursively(parser, list);
        return list;
    }

    private static void splitTypeNamesRecursively(TypeParser parser, ICollection<string> list)
    {
        if (parser.GenericParameters == null || parser.GenericParameters.Count == 0)
        {
            list.Add(parser.FullyQualifiedTypeName);
            return;
        }

        foreach (var innerParser in parser.GenericParameters)
        {
            splitTypeNamesRecursively(innerParser, list);
        }
    }
}