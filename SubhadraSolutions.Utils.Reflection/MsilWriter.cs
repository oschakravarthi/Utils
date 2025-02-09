using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Reflection;

internal static class MsilWriter
{
    private static readonly Dictionary<Type, MethodInfo> _emitMethods;

    static MsilWriter()
    {
        _emitMethods = [];
        var allMethods = typeof(ILGenerator).GetMethods();
        foreach (var method in allMethods)
        {
            if (method.Name == "Emit")
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 2)
                {
                    _emitMethods.Add(parameters[1].ParameterType, method);
                }
            }
        }
    }

    internal static DynamicMethod BuildDynamicMethod(MethodInformation info)
    {
        var returnType = CoreReflectionHelper.GetType(info.ReturnType);
        var parameterTypes = getParameterTypes(info.ParameterTypes);
        var dynamicMethod = new DynamicMethod("DynamicMethod" + GeneralHelper.Identity,
            CoreReflectionHelper.GetType(info.ReturnType), parameterTypes, typeof(MsilWriter));

        var ilGen = dynamicMethod.GetILGenerator();
        buildMethodBody(ilGen, info);
        return dynamicMethod;
    }

    private static void buildMethodBody(ILGenerator ilGen, MethodInformation info)
    {
        foreach (var local in info.LocalVariableTypes)
        {
            ilGen.DeclareLocal(CoreReflectionHelper.GetType(local));
        }

        foreach (var instruction in info.Instructions)
        {
            if (instruction.Data == null)
            {
                if (instruction.TypeName == null)
                {
                    ilGen.Emit(instruction.OpCode);
                }
                else
                {
                    var classType = CoreReflectionHelper.GetType(instruction.TypeName);
                    if (instruction.MethodCallInfo == null)
                    {
                        _emitMethods[typeof(Type)].Invoke(ilGen, [instruction.OpCode, classType]);
                    }
                    else
                    {
                        var pTypes = getParameterTypes(instruction.MethodCallInfo.ParameterTypes);
                        if (instruction.MethodCallInfo.MethodName == ".ctor")
                        {
                            var ci = classType.GetConstructor(pTypes);
                            _emitMethods[typeof(ConstructorInfo)]
                                .Invoke(ilGen, [instruction.OpCode, ci]);
                        }
                        else
                        {
                            var mi = classType.GetMethod(instruction.MethodCallInfo.MethodName, pTypes);
                            _emitMethods[typeof(MethodInfo)].Invoke(ilGen, [instruction.OpCode, mi]);
                        }
                    }
                }
            }
            else
            {
                var type = instruction.Data.GetType();
                if (!_emitMethods.TryGetValue(type, out var mi))
                {
                    foreach (var kvp in _emitMethods)
                    {
                        if (kvp.Key.IsAssignableFrom(type))
                        {
                            mi = kvp.Value;
                            break;
                        }
                    }
                }

                if (mi == null)
                {
                }

                mi.Invoke(ilGen, [instruction.OpCode, instruction.Data]);
            }
        }
    }

    private static Type[] getParameterTypes(IList<string> parameterTypes)
    {
        var result = new Type[parameterTypes.Count];
        for (var i = 0; i < parameterTypes.Count; i++) result[i] = CoreReflectionHelper.GetType(parameterTypes[i]);

        return result;
    }
}