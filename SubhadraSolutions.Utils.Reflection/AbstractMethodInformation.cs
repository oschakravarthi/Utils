using System;
using System.Reflection;

namespace SubhadraSolutions.Utils.Reflection;

[Serializable]
public abstract class AbstractMethodInformation
{
    internal string[] ParameterTypes { get; set; }

    internal static void PopulateMethodBaseInformation(MethodBase method, AbstractMethodInformation info)
    {
        var parameters = method.GetParameters();
        info.ParameterTypes = new string[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
            info.ParameterTypes[i] = parameters[i].ParameterType.AssemblyQualifiedName;
    }
}