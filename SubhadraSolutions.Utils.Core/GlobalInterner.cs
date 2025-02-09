using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils;

public static class GlobalInterner<T>
{
    private static Action<T> _delegate;

    static GlobalInterner()
    {
        BuildDynamicMethod();
    }

    public static T InternFluent(T a)
    {
        _delegate(a);
        return a;
    }

    public static void InternItems(IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            InternObject(obj);
        }
    }

    public static T1 InternItemsFluent<T1>(T1 objects) where T1 : IEnumerable<T>
    {
        foreach (var obj in objects)
        {
            InternObject(obj);
        }

        return objects;
    }

    public static void InternObject(T a)
    {
        _delegate(a);
    }

    public static IEnumerable<T> WrapIntern(IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            InternObject(obj);
            yield return obj;
        }
    }

    [ILEmitter]
    private static void BuildDynamicMethod()
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertiesCount = properties.Length;
        var dynamicMethod = new DynamicMethod("globalIntenrDynamicMethod", typeof(void), [type],
            typeof(GlobalInterner<T>));
        var ilGen = dynamicMethod.GetILGenerator();

        ilGen.DeclareLocal(typeof(string));
        ilGen.DeclareLocal(typeof(bool));

        for (var i = 0; i < propertiesCount; i++)
        {
            var property = properties[i];

            if (property.PropertyType != typeof(string))
            {
                continue;
            }

            if (property.GetCustomAttribute<NotInternableAttribute>(true) != null)
            {
                continue;
            }

            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Callvirt, property.GetGetMethod());
            ilGen.Emit(OpCodes.Stloc, 0);

            ilGen.Emit(OpCodes.Ldloc, 0);
            ilGen.Emit(OpCodes.Ldnull);
            ilGen.Emit(OpCodes.Cgt_Un);
            ilGen.Emit(OpCodes.Stloc, 1);

            ilGen.Emit(OpCodes.Ldloc, 1);
            var label = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brfalse, label);

            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Ldloc, 0);
            ilGen.Emit(OpCodes.Call, SharedReflectionInfo.StringInternMethod);
            ilGen.Emit(OpCodes.Callvirt, property.GetSetMethod());

            ilGen.MarkLabel(label);
        }

        ilGen.Emit(OpCodes.Ret);
        _delegate = (Action<T>)dynamicMethod.CreateDelegate(typeof(Action<T>));
    }
}